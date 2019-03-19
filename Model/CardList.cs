using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Kanbanboard.Model
{    
    public class CardList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        public uint Position { get; set;}
        [Required] public string BoardId { get; set; }
        [Required] public string Title { get; set; }

        public virtual Board Board { get; set; }        
        public virtual ICollection<Card> Cards { get; set; }
    }
}