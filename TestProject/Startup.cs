using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using TestProject.Datasets;
using TestProject.Models;
using TestProject.Services.Classes;
using TestProject.Services.Interfaces;

namespace TestProject
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public IHostingEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IHostingEnvironment environment)
    {
      this.Configuration = configuration;

      this.Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      string connectionString = Configuration["dbConnect:DefaultConnection"];

      services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

      #region Identity
      services.AddIdentity<User, IdentityRole>(options =>
      {
        options.User = new UserOptions
        {
          RequireUniqueEmail = true
        };

        options.Password = new PasswordOptions
        {
          RequireDigit = true,
          RequireNonAlphanumeric = false,
          RequireUppercase = false,
          RequireLowercase = true,
          RequiredLength = 5,
        };
      }).AddEntityFrameworkStores<ApplicationContext>();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(cfg =>
          {
            cfg.SaveToken = true;

            cfg.TokenValidationParameters = new TokenValidationParameters()
            {
              ValidateIssuer = true,
              ValidIssuer = this.Configuration["jwt:ValidIssuer"],

              ValidateAudience = true,
              ValidAudience = this.Configuration["jwt:ValidAudience"],

              ValidateLifetime = true,

              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["jwt:IssuerSigningKey"]))
            };
          });
      #endregion

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "wwwroot/dist";
      });

      services.AddSingleton<IFileService, FileService>(option => new FileService(this.Environment));

      services.AddSingleton<IConfiguration>(this.Configuration);
    }

    public void Configure(IApplicationBuilder app/*, IHostingEnvironment env*/)
    {

      app.UseMvcWithDefaultRoute();
      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
        Path.Combine(this.Environment.WebRootPath, "files")),
        RequestPath = "/files"
      });

      if (this.Environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();

        app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
        {
          HotModuleReplacement = true
        });
      }

      //app.Use(async (context, next) =>
      //{
      //  await next();
      //  if (context.Response.StatusCode == 404 &&
      //     !Path.HasExtension(context.Request.Path.Value) &&
      //     !context.Request.Path.Value.StartsWith("/api/"))
      //  {
      //    context.Request.Path = "/dist/index.html";
      //    await next();
      //  }
      //});

      //app.UseMvcWithDefaultRoute();

      //app.UseDefaultFiles();

      //app.UseStaticFiles();
    }
  }
}
