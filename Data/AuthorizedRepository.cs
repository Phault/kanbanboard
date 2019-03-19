using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kanbanboard.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimplePatch;

namespace Kanbanboard.Data
{
    public class AuthorizedRepository : IRepository
    {
        private IRepository _repository;
        private BoardContext _boardContext;
        private HttpContext _httpContext;

        public AuthorizedRepository(
            IRepository repository,
            BoardContext boardContext, 
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _boardContext = boardContext;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<Board> GetBoardAsync(string id)
        {
            var board = await _repository.GetBoardAsync(id);

            await CheckBoardAccessAsync(id, BoardMemberRole.Viewer);

            return board;
        }

        public async Task<IEnumerable<Board>> GetBoardsOwnedAsync()
        {
            var userId = GetUserId();
            return await _boardContext.BoardMembers
                .Where(m => m.UserId == userId && m.Role > BoardMemberRole.None)
                .Select(m => m.Board)
                .ToListAsync();
        }

        public async Task<Board> CreateBoardAsync(Board board)
        {
            var userId = GetUserId();
            if (board.Members == null || board.Members.Count == 0)
            {
                board.Members = new List<BoardMember> {
                    new BoardMember { 
                        Board = board, 
                        UserId = userId, 
                        Role = BoardMemberRole.Admin
                    }
                };
            }
            return await _repository.CreateBoardAsync(board);
        }

        public async Task<Board> UpdateBoardAsync(string id, Delta<Board> patch)
        {
            var board = await _repository.GetBoardAsync(id);

            await CheckBoardAccessAsync(id, BoardMemberRole.Admin);

            patch.Patch(board);
            await _boardContext.SaveChangesAsync();
            return board;
        }

        public async Task DeleteBoardAsync(string id)
        {
            var board = await GetBoardAsync(id);

            await CheckBoardAccessAsync(id, BoardMemberRole.Admin);

            _boardContext.Boards.Remove(board);
            await _boardContext.SaveChangesAsync();
        }

        public async Task<CardList> GetListAsync(string id)
        {
            var list = await _repository.GetListAsync(id);
            
            await CheckBoardAccessAsync(list.BoardId, BoardMemberRole.Viewer);
            
            return list;
        }

        public async Task<CardList> CreateListAsync(CardList list)
        {
            var board = await GetBoardAsync(list.BoardId);

            await CheckBoardAccessAsync(list.BoardId, BoardMemberRole.Normal);
            
            return await _repository.CreateListAsync(list);
        }

        public async Task<CardList> UpdateListAsync(string id, Delta<CardList> patch)
        {
            var list = await GetListAsync(id);

            await CheckBoardAccessAsync(list.BoardId, BoardMemberRole.Normal);

            patch.Patch(list);
            await _boardContext.SaveChangesAsync();
            return list;
        }

        public async Task DeleteListAsync(string id)
        {
            var list = await GetListAsync(id);

            await CheckBoardAccessAsync(list.BoardId, BoardMemberRole.Normal);

            _boardContext.Lists.Remove(list);
            await _boardContext.SaveChangesAsync();
        }

        public async Task<Card> GetCardAsync(string id)
        {
            var card = await _repository.GetCardAsync(id);

            await CheckCardAccessAsync(id, BoardMemberRole.Viewer);
            
            return card;
        }

        public async Task<Card> CreateCardAsync(Card card)
        {
            var list = await GetListAsync(card.ListId);

            await CheckBoardAccessAsync(list.BoardId, BoardMemberRole.Normal);
                
            return await _repository.CreateCardAsync(card);
        }

        public async Task<Card> UpdateCardAsync(string id, Delta<Card> patch)
        {
            var card = await GetCardAsync(id);
            
            await CheckCardAccessAsync(id, BoardMemberRole.Normal);

            patch.Patch(card);
            await _boardContext.SaveChangesAsync();
            return card;
        }

        public async Task DeleteCardAsync(string id)
        {
            var card = await GetCardAsync(id);

            await CheckCardAccessAsync(id, BoardMemberRole.Normal);

            _boardContext.Cards.Remove(card);
            await _boardContext.SaveChangesAsync();
        }

        public Task<string> GetBoardIdFromCardAsync(string cardId)
        {
            return _repository.GetBoardIdFromCardAsync(cardId);
        }

        public async Task<BoardMemberRole> GetRoleAsync(string boardId)
        {
            var userId = GetUserId();
            var boardMember = await _boardContext.BoardMembers.FindAsync(boardId, userId);
            return boardMember?.Role ?? BoardMemberRole.None;
        }

        public async Task<BoardMemberRole> GetRoleFromCardAsync(string cardId)
        {
            var boardId = await GetBoardIdFromCardAsync(cardId);
            return await GetRoleAsync(boardId);
        }

        private string GetUserId()
        {
            var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId == null)
                throw new InvalidOperationException("user is unauthenticated");
            
            return userId;
        }

        public async Task CheckBoardAccessAsync(string boardId, BoardMemberRole minimumRole)
        {
            var role = await GetRoleAsync(boardId);
            if (role < minimumRole)
                throw new InvalidOperationException($"user is not authorized to change board '{boardId}'");
        }

        public async Task CheckCardAccessAsync(string cardId, BoardMemberRole minimumRole)
        {
            var boardId = await GetBoardIdFromCardAsync(cardId);
            await CheckBoardAccessAsync(boardId, minimumRole);
        }
    }
}