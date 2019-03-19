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
    public class CardsController : ControllerBase
    {
        private IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHubContext<BoardHub, IBoardClient> _hub;

        public CardsController(AuthorizedRepository repository, IMapper mapper, IHubContext<BoardHub, IBoardClient> hub)
        {
            _repository = repository;
            _mapper = mapper;
            _hub = hub;
        }

        [HttpGet("{id}", Name = "GetCard")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CardViewModel>> Get(string id)
        {
            var card = await _repository.GetCardAsync(id);

            if (card == null)
                return NotFound();
            
            return _mapper.Map<CardViewModel>(card);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            try {
                var card = await _repository.GetCardAsync(id);
                var boardId = await _repository.GetBoardIdFromCardAsync(id);

                await _repository.DeleteCardAsync(id);

                _hub.Clients.Group(boardId).CardDeleted(id).DoNotWait();

                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<CardViewModel>> Create([FromBody] CardViewModel card)
        {
            var domainCard = _mapper.Map<Model.Card>(card);

            var createdCard = await _repository.CreateCardAsync(domainCard);
            var createdCardDto = _mapper.Map<CardViewModel>(createdCard);

            var boardId = await _repository.GetBoardIdFromCardAsync(createdCard.Id);
            _hub.Clients.Group(boardId).CardCreated(createdCardDto).DoNotWait();

            return CreatedAtRoute("GetCard", new { id = createdCardDto.Id }, createdCardDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] Delta<CardPatchViewModel> patch)
        {
            var domainPatch = _mapper.Map<Delta<Model.Card>>(patch);

            try {
                var patchedCard = await _repository.UpdateCardAsync(id, domainPatch);

                var boardId = await _repository.GetBoardIdFromCardAsync(id);
                _hub.Clients.Group(boardId).CardChanged(id, patch).DoNotWait();

                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}
