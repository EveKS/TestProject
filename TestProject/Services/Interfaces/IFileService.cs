using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestProject.Models;

namespace TestProject.Services.Interfaces
{
  public interface IFileService
  {
    Task<IFormFileCollection> UppFiles(IFormFileCollection file, List<FilesModel> filesModels, IHostingEnvironment _appEnvironment);
  }
}
