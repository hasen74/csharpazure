using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp.Dtos.User
{
  public class GetUserDto
  {
    public int Id { get; set; }
    public string Email { get; set; } = "user@email.com";
    public RoleClass Role { get; set; } = RoleClass.User;
  }
}