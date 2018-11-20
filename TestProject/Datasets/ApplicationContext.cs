using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestProject.Models;

namespace TestProject.Datasets
{
  public class ApplicationContext: IdentityDbContext<User>
  {
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options)
    {
    }
  }
}
