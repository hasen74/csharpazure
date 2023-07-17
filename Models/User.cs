using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp.Models
{
  public class User
  {
    public int Id { get; set; }
    public string Email { get; set; } = "user@email.com";
    public string PasswordHash { get; set; } = "password";
    public RoleClass Role { get; set; } = RoleClass.User;

  }
}