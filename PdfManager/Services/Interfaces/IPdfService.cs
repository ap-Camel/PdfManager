using PdfManager.Models;

namespace PdfManager.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] ConvertPdfToPdfA3(IFormFile pdfFile);
        byte[] SignPdf(PdfSignRequestModel data);
    }
}
