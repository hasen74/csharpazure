using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp.Dtos.User
{
  public class AddUserDto
  {
    public string Email { get; set; } = "user@email.com";
    public string Password { get; set; } = "password";
    public RoleClass Role { get; set; } = RoleClass.User;
  }
}