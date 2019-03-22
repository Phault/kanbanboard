using System.Collections.Generic;

namespace Kanbanboard.ViewModels
{
    public class BoardViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Background { get; set; }
        public ICollection<CardListViewModel> Lists { get; set; }
        public ICollection<BoardMemberViewModel> Members { get; set; }
    }
}