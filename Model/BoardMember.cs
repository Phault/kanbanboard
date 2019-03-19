using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Kanbanboard.Model
{
    public enum BoardMemberRole
    {
        None = -1,
        Viewer,
        Normal,
        Admin,
    }

    public class BoardMember
    {
        [Key] public string BoardId { get; set; }
        [Key] public string UserId { get; set; }
        [Required] public BoardMemberRole Role { get; set;}

        public virtual Board Board { get; set; }
        public virtual AppUser User { get; set; }
    }
}