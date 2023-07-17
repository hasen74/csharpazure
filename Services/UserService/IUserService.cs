using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp.Services.UserService
{
  public interface IUserService
  {
    Task<ServiceResponse<List<GetUserDto>>> GetAllUsers();
    Task<ServiceResponse<GetUserDto>> GetUserById(int id);
    Task<ServiceResponse<GetUserDto>> AddUser(AddUserDto newUser);
    Task<ServiceResponse<GetUserDto>> UpdateUser(UpdateUserDto updatedUser, int id);
    Task<ServiceResponse<GetUserDto>> DeleteUser(int id);
  }
}