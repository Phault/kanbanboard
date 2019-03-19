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
    public class ListsController : ControllerBase
    {
        private IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHubContext<BoardHub, IBoardClient> _hub;
        private readonly CardsController _cardsController;

        public ListsController(AuthorizedRepository repository, 
            IMapper mapper, 
            IHubContext<BoardHub, IBoardClient> hub,
            CardsController cardsController)
        {
            _repository = repository;
            _mapper = mapper;
            _hub = hub;
            _cardsController = cardsController;
        }

        [HttpGet("{id}", Name = "GetList")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CardListViewModel>> Get(string id)
        {
            var list = await _repository.GetListAsync(id);

            if (list == null)
                return NotFound();
            
            return _mapper.Map<CardListViewModel>(list);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            try {
                var list = await _repository.GetListAsync(id);

                await _repository.DeleteListAsync(id);

                _hub.Clients.Group(list.BoardId).ListDeleted(id).DoNotWait();

                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<CardListViewModel>> Create([FromBody] CardListViewModel list)
        {
            var createdList = await _repository.CreateListAsync(_mapper.Map<CardList>(list));
            var createdListDto = _mapper.Map<CardListViewModel>(createdList);

            _hub.Clients.Group(createdList.BoardId).ListCreated(createdListDto).DoNotWait();

            return CreatedAtRoute("GetList", new { id = createdListDto.Id }, createdListDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] Delta<CardListPatchViewModel> patch)
        {
            var domainPatch = _mapper.Map<Delta<CardList>>(patch);

            try {
                var patchedList = await _repository.UpdateListAsync(id, domainPatch);

                _hub.Clients.Group(patchedList.BoardId).ListChanged(id, patch).DoNotWait();

                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/cards")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public Task<ActionResult<CardViewModel>> CreateCardInList(string id, [FromBody] CardViewModel card)
        {
            card.ListId = id;
            return _cardsController.Create(card);
        }
    }
}
