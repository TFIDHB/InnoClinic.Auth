using AutoMapper;
using InnoClinic.Auth.API.BLL.DTOs;
using InnoClinic.Auth.API.DAL.Entities;

namespace InnoClinic.Auth.API.BLL.AutoMapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<RegisterRequestDto, User>()
                .ForMember(p => p.PasswordHash, o => o.Ignore());
            CreateMap<User, UserAccountInfoDto>();
            CreateMap<UpdateUserAccountInfoDto, User>();
        }
    }
}
