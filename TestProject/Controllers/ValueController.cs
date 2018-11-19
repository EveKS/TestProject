using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TestProject.Controllers
{
  [Route("api/[controller]")]
  public class ValuesController : Controller
  {
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "43534534", "43534534", "43534534" };
    }
  }
}
