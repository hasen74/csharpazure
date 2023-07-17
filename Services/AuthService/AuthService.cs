using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace csharp.Services.AuthService
{
  public class AuthService : IAuthService
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public AuthService(DataContext context, IMapper mapper, IAuthorizationService authorizationService)
    {
      _context = context;
      _mapper = mapper;
    }
    public async Task<ServiceResponse<String>> Authenticate(AuthUserDto userIdentity)
    {
      var serviceResponse = new ServiceResponse<String>();

      // Validate email format
      if (!IsValidEmail(userIdentity.Email))
        throw new Exception("Invalid email format.");

      try
      {
        // Get user from database and check if it exists
        var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(userIdentity.Email.ToLower()));
        if (user is null)
          throw new Exception($"User with email '{userIdentity.Email}' not found.");

        // Check if password is correct
        if (!BCrypt.Net.BCrypt.Verify(userIdentity.Password, user.PasswordHash))
          throw new Exception("Incorrect password.");

        try
        {
          // Connect to Azure Key Vault
          var credential = new DefaultAzureCredential();
          var client = new SecretClient(new Uri("https://ticwin1-vault.vault.azure.net/"), credential);

          try
          {
            // Get secret from Azure Key Vault
            KeyVaultSecret secret = await client.GetSecretAsync("TIC-WIN1-KEY");
            string secretKey = secret.Value;

            // Generate JWT token
            var token = GenerateToken(user.Email, user.Id, user.Role, secretKey);

            serviceResponse.Data = token;
            serviceResponse.Message = "User authenticated successfully.";
          }
          catch (Azure.RequestFailedException ex)
          {
            // Azure Key Vault errors handling
            serviceResponse.Success = false;
            serviceResponse.Message = $"Failed to retrieve secret from Azure Key Vault: {ex.Message}";
          }
        }
        catch (Azure.Identity.AuthenticationFailedException ex)
        {
          // Authentication to Azure Key Vault errors handling
          serviceResponse.Success = false;
          serviceResponse.Message = $"Failed to authenticate with Azure Key Vault: {ex.Message}";
        }
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }

      return serviceResponse;
    }

    private bool IsValidEmail(string email)
    {
      const string emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
      return Regex.IsMatch(email, emailPattern);
    }

    private string GenerateToken(string email, int id, RoleClass role, string secretKey)
    {
      List<Claim> claims = new List<Claim> {
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role.ToString()),
        new Claim(ClaimTypes.Actor, id.ToString()),
        // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: "dotnet-user-jwts",
          audience: "TIC-WIN1_USERS",
          claims: claims,
          expires: DateTime.UtcNow.AddHours(24),
          signingCredentials: credentials
      );

      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

      Console.WriteLine(tokenString);

      return tokenString;
    }
  }
}