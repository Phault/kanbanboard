using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kanbanboard.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kanbanboard.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(BoardContext context, UserManager<AppUser> userManager)
        {
            context.Database.Migrate();

            if (context.Boards.Any())
                return;

            var user = new AppUser
            {
                UserName = "user",
                Email = "my@email.com",
            };
            await userManager.CreateAsync(user, "password");

            var otherUser = new AppUser
            {
                UserName = "otherUser",
                Email = "myother@email.com",
            };
            await userManager.CreateAsync(otherUser, "password");

            var board = new Board
            {
                Title = "My Todo List",
            };

            var members = new List<BoardMember> {
                new BoardMember { Board = board, User = user, Role = BoardMemberRole.Admin },
                new BoardMember { Board = board, User = otherUser }
            };

            var lists = new List<CardList> {
                new CardList {
                    Title = "To be done"
                },
                new CardList {
                    Title = "Doing"
                },
                new CardList {
                    Title = "Done"
                }
            };

            for (int i = 0; i < lists.Count; i++)
            {
                lists[i].Board = board;
                lists[i].Position = (uint) ushort.MaxValue * (uint)(i + 1);
            }

            var cards = new List<Card> {
                new Card { Title = "Laundry", List = lists[0], Position = ushort.MaxValue},
                new Card { Title = "Grocery shopping", List = lists[0], Position = ushort.MaxValue * 2 },
                new Card { Title = ".NET Project", List = lists[1], Position = ushort.MaxValue },
                new Card { Title = "Procrastinate", List = lists[2], Position = ushort.MaxValue }
            };

            await context.Boards.AddAsync(board);
            await context.BoardMembers.AddRangeAsync(members);
            await context.Lists.AddRangeAsync(lists);
            await context.Cards.AddRangeAsync(cards);
            await context.SaveChangesAsync();
        }
    }
}