using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TestProject.Datasets;
using TestProject.Models;

namespace TestProject
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
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

      services.AddMvc()/*.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)*/;

      //services.AddSpaStaticFiles(configuration =>
      //{
      //  configuration.RootPath = "wwwroot/dist";
      //});

      services.AddSingleton<IConfiguration>(this.Configuration);
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.Use(async (context, next) =>
      {
        await next();
        if (context.Response.StatusCode == 404 &&
           !Path.HasExtension(context.Request.Path.Value) &&
           !context.Request.Path.Value.StartsWith("/api/"))
        {
          context.Request.Path = "/dist/index.html";
          await next();
        }
      });

      app.UseMvcWithDefaultRoute();

      app.UseDefaultFiles();
      app.UseStaticFiles();

      //app.Run(async (context) =>
      //{
      //  context.Response.ContentType = "text/html";
      //  await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "dist/index.html"));
      //});
    }
  }
}
