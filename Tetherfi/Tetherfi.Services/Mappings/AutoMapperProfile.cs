using AutoMapper;
using Tetherfi.Common.Models;
using Tetherfi.Data.Models;

namespace Tetherfi.Services.Mappings;

public class AutoMapperProfile : Profile
{
		public AutoMapperProfile()
    {
        CreateMap<Register, ApplicationUser>();
        CreateMap<ApplicationUserData, User>()
           .ForMember(
               destination => destination.Id,
               options => options.MapFrom(
                   source => source.ApplicationUser.Id
                   )
           )
           .ForMember(
               destination => destination.Email,
               options => options.MapFrom(
                   source => source.ApplicationUser.Email
                   )
           )
           .ForMember(
               destination => destination.FirstName,
               options => options.MapFrom(
                   source => source.ApplicationUser.FirstName
                   )
           )
           .ForMember(
               destination => destination.LastName,
               options => options.MapFrom(
                   source => source.ApplicationUser.LastName
                   )
           )
           .ForMember(
               destination => destination.Username,
               options => options.MapFrom(
                   source => source.ApplicationUser.UserName
                   )
           )
           .ForMember(
               destination => destination.Roles,
               options => options.MapFrom(
                   source => source.Roles
                   )
           )
           .ForMember(
               destination => destination.Dob,
               options => options.MapFrom(
                   source => source.ApplicationUser.Dob
                   )
           ).
           ForMember(
                destination => destination.ProfilePicture,
                options => options.MapFrom(
                    source => source.ApplicationUser.ProfilePicture
                    )
           );
    }
}

