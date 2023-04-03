using AutoMapper;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;

namespace ScavengerHunt.Profiles;

public class HuntProfile : Profile
{
    public HuntProfile()
    {
        CreateMap<Coordinate, CoordinateDto>().ReverseMap();

        CreateMap<Account, AccountDto>().ReverseMap();

        CreateMap<Game, GameDto>().ForMember(dest => dest.Ratings, opt => opt.MapFrom(game => game.Ratings.Count > 0 ? Math.Round(((double)game.Ratings.Sum()/(double)game.Ratings.Count), 1) : 0))
                                .ForMember(dest => dest.Items, opt => opt.MapFrom(g => g.Items.Count));
        CreateMap<Game, GameDetailDto>().ForMember(dest => dest.Ratings, opt => opt.MapFrom(game => game.Ratings.Count > 0 ? Math.Round(((double)game.Ratings.Sum()/(double)game.Ratings.Count), 1) : 0));
        CreateMap<GameCreateDto, Game>();

        CreateMap<GamePlay, GamePlayDto>();

        CreateMap<Item, ItemDto>();
        CreateMap<ItemCreateDto, Item>();

        CreateMap<Team, TeamDto>().ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members.Count));
        CreateMap<TeamCreateDto, Team>();

        CreateMap<User, UserDto>().ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.Games.Count))
        .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams.Count));
    }
}