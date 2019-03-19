using System.Collections.Generic;

namespace Kanbanboard.ViewModels
{
    public class CardListViewModel
    {
        public string Id { get; set; }
        public string BoardId { get; set; }
        public string Title { get; set; }
        public uint Position { get; set; }
        public ICollection<CardViewModel> Cards { get; set; }
    }
}