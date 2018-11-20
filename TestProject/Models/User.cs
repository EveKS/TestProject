using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TestProject.Models
{
  public class User : IdentityUser
  {
    public IList<FilesModel> FilesModel { get; set; }

    public User()
    {
      this.FilesModel = new List<FilesModel>();
    }
  }
}
