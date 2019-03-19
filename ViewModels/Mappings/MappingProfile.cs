using System.Linq;
using AutoMapper;
using Kanbanboard.Model;
using Kanbanboard.ViewModels.Patches;
using SimplePatch;

namespace Kanbanboard.ViewModels.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BoardViewModel, Board>()
                .ReverseMap()
                .ForMember(b => b.Lists, map => map.MapFrom(b => b.Lists.OrderBy(l => l.Position)));
            CreateMap<CardListViewModel, CardList>()
                .ReverseMap()
                .ForMember(l => l.Cards, map => map.MapFrom(l => l.Cards.OrderBy(c => c.Position)));
            CreateMap<CardViewModel, Card>()
                .ReverseMap();
            CreateMap<BoardMemberViewModel, BoardMember>()
                .ReverseMap();

            CreateMap<AppUser, ProfileViewModel>();

            CreateDeltaMap<BoardPatchViewModel, Board>();
            CreateDeltaMap<CardListPatchViewModel, CardList>();
            CreateDeltaMap<CardPatchViewModel, Card>();
        }

        private IMappingExpression<Delta<TSource>, Delta<TDestination>> CreateDeltaMap<TSource, TDestination>() 
            where TSource : class, new()
            where TDestination : class, new()
        {
            return CreateMap<Delta<TSource>, Delta<TDestination>>().ConstructUsing(source => {
                var dest = new Delta<TDestination>();

                foreach (var pair in source)
                    dest[pair.Key] = pair.Value;

                return dest;
            });
        }
    }
}