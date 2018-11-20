using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TestProject.Models;
using TestProject.Models.ViewModel;

namespace TestProject.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  public class FilesController : Controller
  {
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<User> _userManager;

    private readonly SignInManager<User> _signInManager;

    private readonly IConfiguration _configuration;

    public FilesController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
      this._userManager = userManager;

      this._signInManager = signInManager;

      this._roleManager = roleManager;

      this._configuration = configuration;
    }

    // POST api/value/files-upload
    [HttpPost("files-upload"), DisableRequestSizeLimit]
    public async Task<IActionResult> FilesUpload([FromForm]IFormFileCollection files)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var userName = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (string.IsNullOrWhiteSpace(userName))
      {
        return Unauthorized();
      }

      var user = await _userManager.FindByNameAsync(userName);

      if (user == null)
      {
        return Unauthorized();
      }
      
      return Json($"model:{files != null}, file:{files != null}");
    }
  }
}
