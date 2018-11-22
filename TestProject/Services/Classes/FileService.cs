using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestProject.Models;
using TestProject.Services.Interfaces;

namespace TestProject.Services.Classes
{
  public class FileService : IFileService
  {
    private const string FILE_DIRECTORI_NAME = "/files/";

    private readonly IHostingEnvironment _appEnvironment;

    public FileService(IHostingEnvironment appEnvironment)
    {
      this._appEnvironment = appEnvironment;
    }

    async Task<ConcurrentQueue<FilesModel>> IFileService.UppFiles(IFormFileCollection files)
    {
      var filePath = this._appEnvironment.WebRootPath + FILE_DIRECTORI_NAME;

      var filesNames = new DirectoryInfo(filePath).EnumerateFiles()?.Select(f => f.Name).ToList();

      var result = new ConcurrentQueue<FilesModel>();

      foreach (var uploaded in files)
      {
        if (uploaded != null)
        {
          string fileName = this.CreateName(filesNames, uploaded);

          using (var fileStream = new FileStream(filePath + fileName, FileMode.Create))
          {
            await uploaded.CopyToAsync(fileStream);
          }

          result.Enqueue(new FilesModel
          {
            FileName = uploaded.FileName,
            FilePath = FILE_DIRECTORI_NAME + fileName,
            DateAdded = DateTime.Now
          });

          filesNames.Add(fileName);
        }
      }

      return result;
    }

    Task IFileService.DeleteFile(string fileName) => Task.Run(() => {
      fileName = this._appEnvironment.WebRootPath + fileName;

      if (File.Exists(fileName))
      {
        File.Delete(fileName);
      }
    });

    private string CreateName(List<string> filesNames, IFormFile uploaded)
    {
      var fileNameToChar = Enumerable.Range('a', 'z' - 'a').Select(Convert.ToChar).ToArray();

      var fileExtension = Path.GetExtension(uploaded.FileName);

      var fileName = "";

      do
      {
        this.Sorting(fileNameToChar);

      } while (filesNames.Contains(fileName = new string(fileNameToChar) + fileExtension));

      return fileName;
    }

    private void Sorting(IList<char> list)
    {
      Random rnd = new Random();

      for (int i = list.Count - 1; i > 0; i--)
      {
        int j = rnd.Next(0, i + 1);

        char temp = list[i];

        list[i] = list[j];

        list[j] = temp;
      }
    }
  }
}
