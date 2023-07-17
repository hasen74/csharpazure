using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<String>> Authenticate(AuthUserDto userIdentity);
    }
}