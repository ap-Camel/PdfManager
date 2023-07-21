using System.ComponentModel.DataAnnotations;

namespace PdfManager.Models
{
    public class PdfSignRequestModel
    {
        [Required]
        public IFormFile PdfFile { get; set; }
        [Required]
        public IFormFile CertificateFile { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public bool author { get; set; }
    }
}
