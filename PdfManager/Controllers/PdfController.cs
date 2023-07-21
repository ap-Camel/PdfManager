using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Tls;
using PdfManager.Models;
using PdfManager.Services.Implmentations;
using PdfManager.Services.Interfaces;
using PdfManager.Verifications;
using Swashbuckle.Swagger.Annotations;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PdfManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        public PdfController(IPdfService pdfService)
        {
            _pdfService = pdfService;   
        }

        //[HttpPost("sign-pdf")]
        //public IActionResult SignPdf(PdfSignRequestModel data)
        //{
        //    ValidateSignRequest.Validate(data);

        //    var signedPDF = _pdfService.SignPdf(data);
        //    return File(signedPDF, "application/pdf", "signed.pdf");
        //}

        [HttpPost("sign-pdf")]
        [Description("signs provided pdf document with provided certificate")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, type: typeof(FileContentResult), description: "pdf was signed successfully")]
        public IActionResult SignPdf(IFormFile pdfFile, IFormFile certificate, string password, bool author)
        {
            var data = new PdfSignRequestModel { PdfFile = pdfFile, CertificateFile = certificate, password = password, author = author };
            ValidateSignRequest.Validate(data);

            var signedPDF = _pdfService.SignPdf(data);
            return File(signedPDF, "application/pdf", "signed.pdf");
        }
    }
}
