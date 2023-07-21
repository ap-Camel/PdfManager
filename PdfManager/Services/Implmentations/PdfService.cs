using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using PdfManager.Models;
using PdfManager.Services.Interfaces;
using System.Collections.ObjectModel;

namespace PdfManager.Services.Implmentations
{
    public class PdfService : IPdfService
    {
        public byte[] SignPdf(PdfSignRequestModel data)
        {            
            string signtureReason = "signing Pdf";
            string location = "my location";
            int signCount = 0;
            int xAxis = 0, width = 100, pageCount = 0;

            //ICollection<byte> bytes = new Collection<byte>();

            using (var pdfStream = data.PdfFile.OpenReadStream())
            //using (var pdfMemoryStream = new MemoryStream())
            {
                //PdfWriter writer = new PdfWriter(pdfMemoryStream);
                PdfReader reader = new PdfReader(pdfStream);
                //PdfDocument pdfDoc = new PdfDocument(reader, writer);
                PdfDocument pdfDoc = new PdfDocument(reader);
                pageCount = pdfDoc.GetNumberOfPages();
                SignatureUtil signUtil = new SignatureUtil(pdfDoc);
                signCount = signUtil.GetSignatureNames().Count;
                xAxis = ((signCount % 2) * width) + 5;

                //if (signCount > 2)
                //    pdfDoc.AddNewPage();

                //pageCount = pdfDoc.GetNumberOfPages();
                //bytes = pdfMemoryStream.ToArray();
            }

            using (var cert = data.CertificateFile.OpenReadStream())
            using (var pdfStream = data.PdfFile.OpenReadStream())
            using (var signedMemoryStream = new MemoryStream())
            {
                Pkcs12Store pk12 = new Pkcs12Store(cert, "password".ToCharArray());
                string alias = null;
                foreach (var a in pk12.Aliases)
                {
                    alias = ((string)a);
                    if (pk12.IsKeyEntry(alias))
                        break;
                }

                ICipherParameters pk = pk12.GetKey(alias).Key;
                X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
                X509Certificate[] chain = new X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                {
                    chain[k] = ce[k].Certificate;
                }

                PdfReader reader = new PdfReader(pdfStream);
                PdfSigner signer = new PdfSigner(reader, signedMemoryStream, new StampingProperties().UseAppendMode());

                //var location2 = chain[0].IssuerDN.GetValueList()[2];

                // will this work if the certificate uses different encryption?
                IExternalSignature pks = new PrivateKeySignature(new PrivateKeyBC(pk), DigestAlgorithms.SHA256);

                IX509Certificate[] certificateWrappers = new IX509Certificate[chain.Length];
                for (int i = 0; i < certificateWrappers.Length; ++i)
                {
                    certificateWrappers[i] = new X509CertificateBC(chain[i]);
                }

                //Signture appearance
                iText.Kernel.Geom.Rectangle rect = new iText.Kernel.Geom.Rectangle(xAxis, 36, width, 50);
                PdfSignatureAppearance appearance = signer.GetSignatureAppearance()
                    .SetReason(signtureReason)
                    .SetLocation(location)
                    .SetReuseAppearance(false)
                    .SetPageRect(rect)
                    .SetPageNumber(pageCount)
                    .SetLayer2Font(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetLayer2FontSize(12)
                    .SetLayer2FontColor(ColorConstants.BLACK)
                    .SetCertificate(certificateWrappers[0]);


                signer.SetSignDate(DateTime.Now);
                signer.SetFieldName(Guid.NewGuid().ToString());
                signer.SetCertificationLevel(data.author ? PdfSigner.CERTIFIED_FORM_FILLING : PdfSigner.NOT_CERTIFIED);

                signer.SignDetached(pks, certificateWrappers, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
                return signedMemoryStream.ToArray();
            }
        }
    }
}
