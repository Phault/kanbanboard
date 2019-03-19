using System.Threading.Tasks;
using Kanbanboard.ViewModels;
using Kanbanboard.ViewModels.Patches;
using SimplePatch;

namespace Kanbanboard.Hubs
{
    public interface IBoardClient
    {
        Task BoardChanged(string id, Delta<BoardPatchViewModel> delta);
        Task BoardDeleted(string id);

        Task ListCreated(CardListViewModel list);
        Task ListChanged(string id, Delta<CardListPatchViewModel> delta);
        Task ListDeleted(string id);

        Task CardCreated(CardViewModel card);
        Task CardChanged(string id, Delta<CardPatchViewModel> delta);
        Task CardDeleted(string id);
    }
}