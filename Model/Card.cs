using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Kanbanboard.Model
{    
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        public uint Position { get; set;}
        [Required] public string ListId { get; set; }
        [Required] public string Title { get; set; }
        public string Description { get; set; }

        public virtual CardList List { get; set; }
    }
}