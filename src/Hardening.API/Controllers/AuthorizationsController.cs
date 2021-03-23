using System;
using System.Threading.Tasks;
using Hardening.API.Models.Requests;
using Hardening.API.Models.Responses;
using Hardening.API.Services;
using Hardening.Models.Domains;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hardening.API.Controllers
{
  [ApiController]
  [Route("api/authorizations")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class AuthorizationsController : ControllerBase
  {
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtHandler _jwtHandler;

    public AuthorizationsController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IJwtHandler jwtHandler)
    {
      _roleManager = roleManager;
      _userManager = userManager;
      _jwtHandler = jwtHandler;
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> SigninAsync(SignInRequest request)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var user = await _userManager.FindByNameAsync(request.Username);
      if (user == null) return NotFound(new Exception("Current user not found."));
      var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.Password);
      if (checkPasswordResult)
      {
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHandler.CreateWithJwtPayload(user, roles);
        var response = new SignInResponse
        {
          AccessToken = token.Token,

          Username = user.UserName,
          RoleName = string.Join(',', roles),
          ExpireOn = token.ExpireOn
        };

        return Ok(response);
      }

      return BadRequest("Username or Password miss match.");
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync(SignUpRequest request)
    {
      var existing = await _userManager.FindByEmailAsync(request.Email);
      if (existing != null) return BadRequest(new Exception("Current email is existing."));
      if (request.Password != request.ConfirmPassword) return BadRequest(new Exception("Password miss match."));

      var nUser = new ApplicationUser
      {
        UserName = request.Username,
        Firstname = request.Firstname,
        Lastname = request.Lastname,
        Email = request.Email
      };

      var createUserResult = await _userManager.CreateAsync(nUser, request.Password);
      if (createUserResult.Succeeded)
      {
        var existingRole = await _roleManager.FindByNameAsync("Admin");
        if (existingRole != null)
        {
          var addUserRole = await _userManager.AddToRoleAsync(nUser, existingRole.Name);
          if (addUserRole.Succeeded)
          {
            return Ok(new SignUpResponse
            {
              Email = request.Email,
              IsSuccess = true
            });
          }

          return BadRequest(new SignUpResponse
          {
            Email = request.Email,
            IsSuccess = false,
            Message = "Cannot add current user to role"
          });
        }

        return BadRequest(new SignUpResponse
        {
          Email = request.Email,
          IsSuccess = false,
          Message = "Cannot find existing role."
        });
      }

      return BadRequest(new SignUpResponse
      {
        Email = request.Email,
        IsSuccess = false,
        Message = "Cannot create user."
      });
    }
  }
}