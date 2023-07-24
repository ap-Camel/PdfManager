using System.ComponentModel.DataAnnotations;

namespace PdfManager.Models
{
    public class PdfSignRequestModel
    {
        [Required]
        public IFormFile PdfFile { get; set; }
        [Required]
        public IFormFile CertificateFile { get; set; }
        
        public string? password { get; set; } = string.Empty;
        [Required]
        public bool author { get; set; }
    }
}
