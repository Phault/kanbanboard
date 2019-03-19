using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Kanbanboard.Model
{    
    public class AppUser : IdentityUser
    {
        public virtual ICollection<Board> Boards { get; set; }
    }
}