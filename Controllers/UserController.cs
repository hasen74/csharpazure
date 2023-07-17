using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp.Controllers
{
  [ApiController]
  [Route("users")]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public UserController(IUserService userService, IAuthorizationService authorizationService)
    {
      _userService = userService;
      _authorizationService = authorizationService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GetUserDto>>>> Get()
    {
      return Ok(await _userService.GetAllUsers());
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetUserDto>>> GetSingle(int id)
    {
      return Ok(await _userService.GetUserById(id));
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<ServiceResponse<List<GetUserDto>>>> AddUser([FromBody] AddUserDto newUser)
    {
      return Ok(await _userService.AddUser(newUser));
    }

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<ActionResult<ServiceResponse<List<GetUserDto>>>> UpdateUser([FromBody] UpdateUserDto updatedUser, int id)
    {
      var response = await _userService.UpdateUser(updatedUser, id);
      if (response.Data is null)
      {
        return NotFound(response);
      }
      return Ok(response);
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<ServiceResponse<GetUserDto>>> DeleteUser(int id)
    {
      var user = await _userService.GetUserById(id);
      // Resource-base authorisation middleware
      // Check if authenticated user is admin or the same one as the target
      var result = await _authorizationService.AuthorizeAsync(User, user.Data, "SamePersonOrAdminPolicy");
      if (!result.Succeeded)
      {
        return new ForbidResult();
      }
      var response = await _userService.DeleteUser(id);
      return Ok(response);
    }
  }
}