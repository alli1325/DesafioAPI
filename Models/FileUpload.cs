using Microsoft.AspNetCore.Http;

namespace DesafioAPI.Models
{
    public class FileUpload
    {
        public IFormFile Foto { get; set; }
    }
}