using Kanbanboard.Model;

namespace Kanbanboard.ViewModels
{
    public class BoardMemberViewModel
    {
        public string UserId { get; set; }
        public BoardMemberRole Role { get; set; }
    }
}