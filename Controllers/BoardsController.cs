using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kanbanboard.Data;
using Kanbanboard.Hubs;
using Kanbanboard.Model;
using Kanbanboard.Utils.Extensions;
using Kanbanboard.ViewModels;
using Kanbanboard.ViewModels.Patches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SimplePatch;

namespace Kanbanboard.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/")]
    public class BoardsController : ControllerBase
    {
        private IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHubContext<BoardHub, IBoardClient> _hub;
        private readonly ListsController _listsController;

        public BoardsController(AuthorizedRepository repository, 
            IMapper mapper, 
            IHubContext<BoardHub, IBoardClient> hub,
            ListsController listsController)
        {
            _repository = repository;
            _mapper = mapper;
            _hub = hub;
            _listsController = listsController;
        }
 
        [HttpGet("{id}", Name = "GetBoard")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BoardViewModel>> Get(string id)
        {
            try 
            {
                var board = await _repository.GetBoardAsync(id);
                return _mapper.Map<BoardViewModel>(board);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            try 
            {
                await _repository.DeleteBoardAsync(id);

                _hub.Clients.Group(id).BoardDeleted(id).DoNotWait();

                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403);
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<BoardViewModel>> Create([FromBody] BoardViewModel board)
        {
            try
            {
                var domainBoard = _mapper.Map<Board>(board);
                await _repository.CreateBoardAsync(domainBoard);

                var createdBoardDto = _mapper.Map<BoardViewModel>(domainBoard);
                return CreatedAtRoute("GetBoard", new { id = createdBoardDto.Id }, createdBoardDto);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] Delta<BoardPatchViewModel> patch)
        {
            try {
                var domainPatch = _mapper.Map<Delta<Board>>(patch);
                await _repository.UpdateBoardAsync(id, domainPatch);
                _hub.Clients.Group(id).BoardChanged(id, patch).DoNotWait();
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403);
            }
        }

        [HttpPost("{id}/members")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CardListViewModel>> GetMembersInBoard(string id)
        {
            try 
            {
                var board = await _repository.GetBoardAsync(id);
                var members = _mapper.Map<IEnumerable<BoardMemberViewModel>>(board.Members);
                return Ok(members);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403);
            }
        }

        [HttpPost("{boardId}/lists")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public Task<ActionResult<CardListViewModel>> CreateListInBoard(string boardId, [FromBody] CardListViewModel list)
        {
            list.BoardId = boardId;
            return _listsController.Create(list);
        }
    }
}
