using PdfManager.Models;

namespace PdfManager.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] ConvertPdfToPdfA3(IFormFile pdfFile);
        byte[] SignPdf(PdfSignRequestModel data);
        byte[] AddEmptyPage(IFormFile PdfFile);
        byte[] AddImage(IFormFile PdfFile, SignatureBox signatureBox);
        //byte[] signPDFTemp(IFormFile pdfFile, IFormFile certificate);
    }
}
