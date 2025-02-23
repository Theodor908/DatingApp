using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDTO>()
        // set age (without querying the whole database) and the main photo for each member
        .ForMember( getTo => getTo.Age, options => options.MapFrom( user => user.DateOfBirth.CalculateAge()))
        .ForMember( getTo => getTo.PhotoUrl, options => options.MapFrom( user => user.Photos.FirstOrDefault( photo => photo.IsMain)!.Url));
        CreateMap<Photo, PhotoDTO>();
        CreateMap<MemberUpdateDTO, AppUser>();
        CreateMap<RegisterDTO, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
    }
}
