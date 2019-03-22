using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Kanbanboard.Model
{    
    public class Board
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required] public string Title { get; set; }

        public string Background { get; set; }
        
        public virtual ICollection<CardList> Lists { get; set; }
        public virtual ICollection<BoardMember> Members { get; set; }
    }
}