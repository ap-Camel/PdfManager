using Org.BouncyCastle.Crypto.Tls;
using PdfManager.Models;

namespace PdfManager.Verifications
{
    public static class ValidateSignRequest
    {
        public static void Validate(PdfSignRequestModel data)
        {
            if (data.PdfFile == null)
            {
                throw new Exception("Pdf file is required");
            }
            if (data.PdfFile.ContentType != "application/pdf")
            {
                throw new Exception("File is not a pdf");
            }
            if (data.CertificateFile == null)
            {
                throw new Exception("certificate file is required");
            }
            if (data.CertificateFile.ContentType != "application/pfx" && data.CertificateFile.ContentType != "application/x-pkcs12")
            {
                throw new Exception("certificate file format is not supported");
            }
        }
    }
}
