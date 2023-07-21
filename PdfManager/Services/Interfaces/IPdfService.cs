using PdfManager.Models;

namespace PdfManager.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] SignPdf(PdfSignRequestModel data);
    }
}
