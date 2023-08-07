using System.ComponentModel.DataAnnotations;

namespace PdfManager.Models
{
    public class PdfSignRequestModel
    {
        /// <summary>
        /// the pdf file to be signed
        /// </summary>
        [Required]
        public IFormFile PdfFile { get; set; }

        /// <summary>
        /// the certificate file used for signing
        /// </summary>
        [Required]
        public IFormFile CertificateFile { get; set; }

        /// <summary>
        /// the password of the signature
        /// </summary>
        public string? password { get; set; } = string.Empty;

        /// <summary>
        /// used for signature level
        /// </summary>
        [Required]
        public bool author { get; set; }

        /// <summary>
        /// info regarding the signature box
        /// </summary>
        public SignatureBox signatureBox { get; set; }

    }
}
