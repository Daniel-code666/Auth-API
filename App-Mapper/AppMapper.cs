using Auth_API.Models;
using Auth_API.Models.Dtos;
using AutoMapper;

namespace Auth_API.App_Mapper
{
    public class AppMapper : Profile
    {
        public AppMapper()
        {
            CreateMap<Users, UserDto>().ReverseMap();
            CreateMap<Users, UserAuthDto>().ReverseMap();
            CreateMap<Users, UserAuthLogin>().ReverseMap();
        }
    }
}
