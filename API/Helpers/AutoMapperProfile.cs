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
            CreateMap<UpdateMemberDto, AppUser>();
            CreateMap<RegisterViewModel, AppUser>();

            CreateMap<Message, MessageDto>()
            .ForMember(dest=>dest.SenderPhotoUrl, memberOptions=>memberOptions.MapFrom
            (src=>src.Sender.Photos.FirstOrDefault(f=>f.IsMain).Url))
            .ForMember(dest=>dest.RecipientPhotoUrl, memberOptions=>memberOptions.MapFrom
            (src=>src.Recipient.Photos.FirstOrDefault(f=>f.IsMain).Url));
        }
    }
}