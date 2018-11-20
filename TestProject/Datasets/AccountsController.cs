using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestProject.Models;
using TestProject.Models.ViewModel.Account;

namespace TestProject.Datasets
{
  [Route("api/[controller]")]
  public class AccountsController : Controller
  {
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<User> _userManager;

    private readonly SignInManager<User> _signInManager;

    private readonly IConfiguration _configuration;

    public AccountsController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
      this._userManager = userManager;

      this._signInManager = signInManager;

      this._roleManager = roleManager;

      this._configuration = configuration;
    }

    // POST api/accounts/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var userIdentity = new User
      {
        Email = model.Email,
        UserName = model.Email
      };

      var result = await this._userManager.CreateAsync(userIdentity, model.Password);

      if (!result.Succeeded)
      {
        return BadRequest();
      }

      return new OkResult();
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = await this._userManager.FindByNameAsync(model.Email);

      if (user == null ||
          !await this._userManager.CheckPasswordAsync(user, model.Password))
      {
        return Unauthorized();
      }

      var claims = await GetValidClaims(user);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["jwt:IssuerSigningKey"]));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var now = DateTime.UtcNow;

      var expiresIn = 30;

      var token = new JwtSecurityToken(this._configuration["jwt:ValidIssuer"],
        this._configuration["jwt:ValidAudience"],
        claims,
        expires: now.Add(TimeSpan.FromDays(expiresIn)),
        signingCredentials: creds);

      return Ok(new
      {
        id = user.Id,
        user_name = user.UserName,
        auth_token = new JwtSecurityTokenHandler().WriteToken(token),
        expires_in = expiresIn
      });
    }

    private async Task<List<Claim>> GetValidClaims(User user)
    {
      IdentityOptions options = new IdentityOptions();

      var claims = new List<Claim>
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
          new Claim(options.ClaimsIdentity.UserNameClaimType, user.UserName)
        };

      var userClaims = await this._userManager.GetClaimsAsync(user);

      var userRoles = await this._userManager.GetRolesAsync(user);

      claims.AddRange(userClaims);

      foreach (var userRole in userRoles)
      {
        claims.Add(new Claim(ClaimTypes.Role, userRole));

        var role = await this._roleManager.FindByNameAsync(userRole);

        if (role != null)
        {
          var roleClaims = await this._roleManager.GetClaimsAsync(role);

          foreach (Claim roleClaim in roleClaims)
          {
            claims.Add(roleClaim);
          }
        }
      }

      return claims;
    }
  }
}
