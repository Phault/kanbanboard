using System;
using System.Threading.Tasks;
using Kanbanboard.Data;
using Kanbanboard.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kanbanboard.Hubs
{
    [Authorize]
    public class BoardHub : Hub<IBoardClient>
    {
        private readonly AuthorizedRepository _repository;

        public BoardHub(AuthorizedRepository repository)
        {
            _repository = repository;
        }

        public async Task SubscribeToBoard(string boardId)
        {
            await _repository.CheckBoardAccessAsync(boardId, BoardMemberRole.Viewer);
            await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
        }

        public async Task UnsubscribeFromBoard(string boardId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);
        }
    }
}