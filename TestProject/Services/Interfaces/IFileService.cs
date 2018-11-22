using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TestProject.Models;

namespace TestProject.Services.Interfaces
{
  public interface IFileService
  {
    Task DeleteFile(string fileName);

    Task<ConcurrentQueue<FilesModel>> UppFiles(IFormFileCollection files);
  }
}
