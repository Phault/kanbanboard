using System.Threading.Tasks;
using Kanbanboard.Model;
using SimplePatch;

namespace Kanbanboard.Data
{
    public interface IRepository
    {
        Task<Board> GetBoardAsync(string id);
        Task<Board> CreateBoardAsync(Board board);
        Task<Board> UpdateBoardAsync(string id, Delta<Board> patch);
        Task DeleteBoardAsync(string id);

        Task<CardList> GetListAsync(string id);
        Task<CardList> CreateListAsync(CardList list);
        Task<CardList> UpdateListAsync(string id, Delta<CardList> patch);
        Task DeleteListAsync(string id);

        Task<Card> GetCardAsync(string id);
        Task<Card> CreateCardAsync(Card card);
        Task<Card> UpdateCardAsync(string id, Delta<Card> patch);
        Task DeleteCardAsync(string id);

        Task<string> GetBoardIdFromCardAsync(string cardId);
    }
}