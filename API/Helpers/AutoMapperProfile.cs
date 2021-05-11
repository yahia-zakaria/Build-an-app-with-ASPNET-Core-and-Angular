using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, MemberDto>()
            .ForMember(dest=>dest.PhotoUrl, memberOptions=>memberOptions.MapFrom(
                src=>src.Photos.FirstOrDefault(src=>src.IsMain).Url
            )).ForMember(dest=>dest.Age, opt=>opt.MapFrom(
                src=>src.DateOfBirth.CalculateAge()
            ));
            CreateMap<Photo, PhotoDto>();
        }
    }
}