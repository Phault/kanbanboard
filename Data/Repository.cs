using System;
using System.Linq;
using System.Threading.Tasks;
using Kanbanboard.Model;
using Microsoft.EntityFrameworkCore;
using SimplePatch;

namespace Kanbanboard.Data
{
    public class Repository : IRepository
    {
        private BoardContext _boardContext;

        public Repository(BoardContext boardContext)
        {
            _boardContext = boardContext;
        }

        public async Task<Board> GetBoardAsync(string id)
        {
            var board = await _boardContext.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
                .Include(b => b.Members)
                .FirstOrDefaultAsync(b => b.Id == id);

            ThrowIfNull(board, id);

            return board;
        }

        public async Task<Board> CreateBoardAsync(Board board)
        {
            _boardContext.Boards.Add(board);
            await _boardContext.SaveChangesAsync();
            return board;
        }

        public async Task<Board> UpdateBoardAsync(string id, Delta<Board> patch)
        {
            var board = await GetBoardAsync(id);
            patch.Patch(board);
            await _boardContext.SaveChangesAsync();
            return board;
        }

        public async Task DeleteBoardAsync(string id)
        {
            var board = new Board() { Id = id };
            _boardContext.Boards.Attach(board);
            _boardContext.Boards.Remove(board);

            try {
                await _boardContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                ThrowIfNull<Board>(null, id);
            }
        }

        public async Task<CardList> GetListAsync(string id)
        {
            var list = await _boardContext.Lists
                .Include(l => l.Cards)
                .FirstOrDefaultAsync(l => l.Id == id);

            ThrowIfNull(list, id);
            
            return list;
        }

        public async Task<CardList> CreateListAsync(CardList list)
        {
            var prevList = await _boardContext.Lists
                .Where(l => l.BoardId == list.BoardId)
                .OrderByDescending(l => l.Position)
                .FirstOrDefaultAsync();

            list.Position = (prevList?.Position ?? 0) + ushort.MaxValue;

            _boardContext.Lists.Add(list);
            await _boardContext.SaveChangesAsync();
            return list;
        }

        public async Task<CardList> UpdateListAsync(string id, Delta<CardList> patch)
        {
            var list = await GetListAsync(id);
            patch.Patch(list);
            await _boardContext.SaveChangesAsync();
            return list;
        }

        public async Task DeleteListAsync(string id)
        {
            var list = await GetListAsync(id);
            _boardContext.Lists.Remove(list);
            await _boardContext.SaveChangesAsync();
        }

        public async Task<Card> GetCardAsync(string id)
        {
            var card = await _boardContext.Cards.FindAsync(id);

            ThrowIfNull(card, id);
            
            return card;
        }

        public async Task<Card> CreateCardAsync(Card card)
        {
            var prevCard = await _boardContext.Cards
                .Where(c => c.ListId == card.ListId)
                .OrderByDescending(c => c.Position)
                .FirstOrDefaultAsync();

            card.Position = (prevCard?.Position ?? 0) + ushort.MaxValue;

            _boardContext.Cards.Add(card);
            await _boardContext.SaveChangesAsync();
            return card;
        }

        public async Task<Card> UpdateCardAsync(string id, Delta<Card> patch)
        {
            var card = await GetCardAsync(id);
            patch.Patch(card);
            await _boardContext.SaveChangesAsync();
            return card;
        }

        public async Task DeleteCardAsync(string id)
        {
            var card = await GetCardAsync(id);
            _boardContext.Cards.Remove(card);
            await _boardContext.SaveChangesAsync();
        }

        public async Task<string> GetBoardIdFromCardAsync(string cardId)
        {
            return await _boardContext.Lists
                .Where(l => l.Id == _boardContext.Cards
                    .Where(c => c.Id == cardId)
                    .Select(c => c.ListId)
                    .First()
                )
                .Select(l => l.BoardId)
                .FirstAsync();
        }

        private void ThrowIfNull<T>(T obj, string id) where T : class
        {
            if (obj == null)
                throw new ArgumentException($"no {nameof(T)} with id '{id}' exists");
        }
    }
}