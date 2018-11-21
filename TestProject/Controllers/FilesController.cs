using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestProject.Datasets;
using TestProject.Models;
using TestProject.Models.ViewModel;
using TestProject.Services.Classes;
using TestProject.Services.Interfaces;

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

    private readonly IFileService _fileService;

    private readonly ApplicationContext _context;

    public FilesController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IFileService fileService,
        ApplicationContext context)
    {
      this._userManager = userManager;

      this._signInManager = signInManager;

      this._roleManager = roleManager;

      this._configuration = configuration;

      this._fileService = fileService;

      this._context = context;
    }

    // POST api/files/files-upload
    [Produces("multipart/form-data")]
    [HttpPost("files-upload"), DisableRequestSizeLimit]
    public async Task<IActionResult> FilesUpload(IFormFileCollection files)
    {
      if (!ModelState.IsValid || files == null)
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

     var fileeData = await this._fileService.UppFiles(files);

      foreach (var item in fileeData)
      {
        item.UserId = user.Id;
      }

      await this._context.AddRangeAsync(fileeData);

      await this._context.SaveChangesAsync();

      IQueryable<FilesModel> filesData =
        this._context.FilesModels.Where(file => file.UserId == user.Id);

      return Json(new {
        Message = "Loaded",
        FilesData = await filesData.ToListAsync()
      });
    }

    // Get api/files/get-files
    [HttpGet("get-files")]
    public async Task<IActionResult> GetFiles()
    {
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

      IQueryable<FilesModel> filesData =
        this._context.FilesModels.Where(file => file.UserId == user.Id);

      return Json(await filesData.ToListAsync());
    }
  }
}
