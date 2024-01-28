using AutoMapper;
using BaseApplication.Entity;
using BaseApplication.Models;

namespace BaseApplication.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserModel, User>().ReverseMap();
        }
    }
}
