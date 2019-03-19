using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kanbanboard.Model;

namespace Kanbanboard.Data
{
    public class BoardContext : IdentityDbContext<AppUser>
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<CardList> Lists { get; set; }
        public DbSet<Model.Card> Cards { get; set; }
        public DbSet<BoardMember> BoardMembers { get; set; }

        public BoardContext(DbContextOptions<BoardContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<BoardMember>()
                .HasKey(bm => new { bm.BoardId, bm.UserId });
        }
    }
}
