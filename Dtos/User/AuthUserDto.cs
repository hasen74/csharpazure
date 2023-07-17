namespace csharp.Dtos.User
{
  public class AuthUserDto
  {
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
  }
}