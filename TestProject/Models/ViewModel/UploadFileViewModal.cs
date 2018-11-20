using Microsoft.AspNetCore.Http;

namespace TestProject.Models.ViewModel
{
  public class UploadFileViewModal
  {
    public IFormFileCollection UploadFiles { get; set; }
  }
}
