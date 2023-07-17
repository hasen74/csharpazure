using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Threading.Tasks;

namespace csharp.Services.UserService
{
  public class UserService : IUserService
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserService(DataContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<ServiceResponse<GetUserDto>> AddUser(AddUserDto newUser)
    {
      var serviceResponse = new ServiceResponse<GetUserDto>();

      try
      {
        if (!IsValidEmail(newUser.Email))
        {
          throw new Exception($"Invalid email format.");
        }

        var user = _mapper.Map<User>(newUser);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        serviceResponse.Data = _mapper.Map<GetUserDto>(dbUser);
        serviceResponse.Message = "New user created successfully.";
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetUserDto>> DeleteUser(int id)
    {
      var serviceResponse = new ServiceResponse<GetUserDto>();

      try
      {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
          throw new Exception($"User with Id '{id}' not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        serviceResponse.Data = _mapper.Map<GetUserDto>(user);
        serviceResponse.Message = "User deleted successfully.";
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }

      return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetUserDto>>> GetAllUsers()
    {
      var serviceResponse = new ServiceResponse<List<GetUserDto>>();
      var dbUsers = await _context.Users.ToListAsync();
      serviceResponse.Data = dbUsers.Select(c => _mapper.Map<GetUserDto>(c)).ToList();
      serviceResponse.Message = "Users found successfully.";
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetUserDto>> GetUserById(int id)
    {
      var serviceResponse = new ServiceResponse<GetUserDto>();
        try
      {

        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (dbUser is null)
          throw new Exception($"User with Id: '{id}' not found.");

        serviceResponse.Data = _mapper.Map<GetUserDto>(dbUser);
        serviceResponse.Message = "User found successfully.";

      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetUserDto>> UpdateUser(UpdateUserDto updatedUser, int id)
    {
      var serviceResponse = new ServiceResponse<GetUserDto>();

      try
      {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
          throw new Exception($"User with Id: '{id}' not found.");

        user.Email = updatedUser.Email;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
        user.Role = updatedUser.Role;

        await _context.SaveChangesAsync();
        serviceResponse.Data = _mapper.Map<GetUserDto>(user);
        serviceResponse.Message = "User updated successfully.";
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
  }
}