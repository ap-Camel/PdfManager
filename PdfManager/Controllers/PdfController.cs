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

        [HttpPost("convert-to-PdfA")]
        [Description("convert the recived pdf to Pdf/A3 standard")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, type: typeof(FileContentResult), description: "pdf was signed successfully")]
        public IActionResult ConvertPDF(IFormFile pdfFile)
        {
            ValidateSignRequest.ValidatePdfOnly(pdfFile);

            var signedPDF = _pdfService.ConvertPdfToPdfA3(pdfFile);
            return File(signedPDF, "application/pdf", "converted.pdf");
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
        public IActionResult SignPdf([FromQuery]SignatureBox signatureBox, IFormFile pdfFile, IFormFile certificate, string password, bool author)
        {
            var data = new PdfSignRequestModel { PdfFile = pdfFile, CertificateFile = certificate, password = password, author = author, signatureBox = signatureBox };
            ValidateSignRequest.Validate(data);

            var signedPDF = _pdfService.SignPdf(data);
            return File(signedPDF, "application/pdf", "signed.pdf");
        }

        [HttpPost("add-new-page")]
        [Description("adds new empty page to the provided pdf")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, type: typeof(FileContentResult), description: "new page was added successfully")]
        public IActionResult AddNewPage(IFormFile pdfFile)
        {
            ValidateSignRequest.ValidatePdfOnly(pdfFile);

            var signedPDF = _pdfService.AddEmptyPage(pdfFile);
            return File(signedPDF, "application/pdf", "converted.pdf");
        }

        //[HttpPost("add-image")]
        //[Description("adds signature image to the provided pdf")]
        //[SwaggerResponse(System.Net.HttpStatusCode.OK, type: typeof(FileContentResult), description: "new page was added successfully")]
        //public IActionResult AddImage(IFormFile pdfFile)
        //{
        //    ValidateSignRequest.ValidatePdfOnly(pdfFile);

        //    var signedPDF = _pdfService.AddImage(pdfFile, new SignatureBox { text = "fff", xAxis = 0, yAxis = 100});
        //    return File(signedPDF, "application/pdf", "withImage.pdf");
        //}
    }
}
