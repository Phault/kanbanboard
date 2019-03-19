namespace Kanbanboard.ViewModels
{
    public class CardViewModel
    {
        public string Id { get; set; }
        public string ListId { get; set; }
        public uint Position { get; set;}
        public string Title { get; set; }
        public string Description { get; set; }
    }
}