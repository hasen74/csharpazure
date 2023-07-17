using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            CreateMap<AuthUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<AddUserDto, User>();
        }
    }
}