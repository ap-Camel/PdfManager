using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Pdfa;
using iText.Signatures;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using PdfManager.Models;
using PdfManager.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PdfManager.Services.Implmentations
{
    public class PdfService : IPdfService
    {

        private readonly string location = "Dotacni portal PK";
        private readonly string signtureReason = "Podpis PDF";
        private readonly int width = 400;
        private readonly int height = 55;

        /// <summary>
        /// converting Pdf file into Pdf/A-3
        /// </summary>
        /// <param name="pdfFile">Pdf file to be converted</param>
        /// <returns>converted Pdf file</returns>
        public byte[] ConvertPdfToPdfA3(IFormFile pdfFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Initialize PDF/A document with output intent
                iText.Pdfa.PdfADocument pdfA = new PdfADocument(
                    new iText.Kernel.Pdf.PdfWriter(memoryStream),
                    iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_3B,
                    new PdfOutputIntent(
                        "Custom",
                        "", "http://www.color.org",
                        "sRGB IEC61966-2.1",
                        new FileStream("Resources/sRGB_CS_profile.icm", FileMode.Open, FileAccess.Read)
                    )
                );

                // Load the source PDF document from the memory stream
                iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfReader(pdfFile.OpenReadStream()));

                // Copy the pages from the source PDF to the PDF/A document
                for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                {
                    PdfPage page = pdfA.AddNewPage();
                    PdfCanvas canvas = new PdfCanvas(page);
                    PdfFormXObject pageCopy = pdfDocument.GetPage(pageNumber).CopyAsFormXObject(pdfA);
                    canvas.AddXObjectAt(pageCopy, 0, 0);
                }

                // Close the document and save changes
                pdfA.Close();
                pdfDocument.Close();

                // Return the byte array of the PDF/A document
                return memoryStream.ToArray();
            }
        }

        public byte[] SignPdf(PdfSignRequestModel data)
        {
            //int signCount = 0;
            int pageCount = 0;

            //byte[] bytes = null;

            //// specifying the position of the signiture depending on number of signitures
            //// adding new page if it's first signiture
            //bool added = false;
            //using (var pdfStream = data.PdfFile.OpenReadStream())
            //using (var pdfMemoryStream = new MemoryStream())
            //{
            //    PdfReader reader = new PdfReader(pdfStream);
            //    PdfDocument pdfDoc = new PdfDocument(reader);

            //    int defaultheight = (int)pdfDoc.GetDefaultPageSize().GetHeight();
            //    pageCount = pdfDoc.GetNumberOfPages();

            //    SignatureUtil signUtil = new SignatureUtil(pdfDoc);
            //    signCount = signUtil.GetSignatureNames().Count;

            //    //xAxis = ((signCount % 4) * width) + 5;
            //    //xAxis = (signCount * width) + 5;
            //    //yAxis = defaultheight - ((signCount * height) + 15) - height;

            //    if(signCount == 0)
            //    {
            //        bytes = AddEmptyPage(data.PdfFile);
            //        pageCount++;
            //        added = true;
            //        //yAxis -= 20;
            //    }
            //}

            using (var pdfStream = data.PdfFile.OpenReadStream())
            {
                PdfReader reader = new PdfReader(pdfStream);
                PdfDocument pdfDoc = new PdfDocument(reader);

                pageCount = pdfDoc.GetNumberOfPages();
            }

            byte[] bytes = AddImage(data.PdfFile, data.signatureBox);

            using (var cert = data.CertificateFile.OpenReadStream())
            //using(Stream pdfStream = added ? new MemoryStream(bytes) : data.PdfFile.OpenReadStream())
            using (Stream pdfStream = data.PdfFile.OpenReadStream())
            //using(Stream pdfStream = new MemoryStream(bytes))
            using (var signedMemoryStream = new MemoryStream())
            {

                // extract certificate alies
                Pkcs12Store pk12 = new Pkcs12Store(cert, data.password.ToCharArray());
                string alias = null;
                foreach (var a in pk12.Aliases)
                {
                    alias = ((string)a);
                    if (pk12.IsKeyEntry(alias))
                        break;
                }

                // extract key store information
                ICipherParameters pk = pk12.GetKey(alias).Key;
                X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
                X509Certificate[] chain = new X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                {
                    chain[k] = ce[k].Certificate;
                }

                PdfReader reader = new PdfReader(pdfStream);
                PdfSigner signer = new PdfSigner(reader, signedMemoryStream, new StampingProperties().UseAppendMode());


                // will this work if the certificate uses different encryption?
                IExternalSignature pks = new PrivateKeySignature(new PrivateKeyBC(pk), DigestAlgorithms.SHA256);

                // extract certifiactes from key store
                IX509Certificate[] certificateWrappers = new IX509Certificate[chain.Length];
                for (int i = 0; i < certificateWrappers.Length; ++i)
                {
                    certificateWrappers[i] = new X509CertificateBC(chain[i]);
                }

                string FONT = "resources/FreeSans.ttf";

                // extract auther's name
                int nameIndex = chain[0].SubjectDN.GetOidList().IndexOf(new DerObjectIdentifier("2.5.4.3"));
                string name = string.Empty;
                if (nameIndex != -1)
                    name = chain[0].SubjectDN.GetValueList()[nameIndex].ToString();

                string textToPrint = data.signatureBox.text.Trim();
                //int textLength = textToPrint.Length;
                //if (textLength >= 35)
                //{
                //    List<char> chars = textToPrint.ToList();
                //    int spaceIndex = 0;
                //    for (int i = 0; i < chars.Count; ++i)
                //    {
                //        if (chars[i] == ' ' && textLength - i < 35)
                //        {
                //            spaceIndex = i;
                //            break;
                //        }
                //    }

                //    if (spaceIndex != 0)
                //    {
                //        chars.Insert(spaceIndex, '\n');
                //        textToPrint = string.Concat(chars);
                //    }
                //}

                ImageData imageData = ImageDataFactory.Create("resources/signature.jpg");

                //Signture appearance
                //iText.Kernel.Geom.Rectangle rect = new iText.Kernel.Geom.Rectangle(data.signatureBox.xAxis + height + 5, data.signatureBox.yAxis, width, height);
                iText.Kernel.Geom.Rectangle rect = new iText.Kernel.Geom.Rectangle(data.signatureBox.xAxis, data.signatureBox.yAxis, width, height);
                PdfSignatureAppearance appearance = signer.GetSignatureAppearance()
                    .SetReason(signtureReason)
                    .SetLocation(location)
                    .SetReuseAppearance(false)
                    .SetPageRect(rect)
                    .SetPageNumber(pageCount)
                    .SetSignatureGraphic(imageData)
                    .SetLayer2Font(PdfFontFactory.CreateFont(FONT))
                    .SetLayer2FontSize(12)
                    .SetLayer2FontColor(ColorConstants.BLACK)
                    .SetLayer2Text(textToPrint)      
                    .SetRenderingMode(PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION)
                    .SetCertificate(certificateWrappers[0]);

                //if (signCount > 4)
                //    appearance.IsInvisible();

                // extra signature information
                signer.SetSignDate(DateTime.Now);
                signer.SetFieldName(Guid.NewGuid().ToString());
                signer.SetCertificationLevel(data.author ? PdfSigner.CERTIFIED_FORM_FILLING : PdfSigner.NOT_CERTIFIED);

                // sign the pdf and return
                signer.SignDetached(pks, certificateWrappers, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
                return signedMemoryStream.ToArray();
            }
        }

        /// <summary>
        /// adding new page to a Pdf document
        /// </summary>
        /// <param name="PdfFile">Pdf file to add new page to</param>
        /// <returns>Pdf file with new page added</returns>
        public byte[] AddEmptyPage(IFormFile PdfFile)
        {
            byte[] data;

            using (var pdfStream = PdfFile.OpenReadStream())
            using (var pdfMemoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(pdfMemoryStream);
                PdfReader reader = new PdfReader(pdfStream);
                PdfDocument pdfDoc = new PdfDocument(reader, writer);

                var page = pdfDoc.AddNewPage();                

                pdfDoc.Close();
                data = pdfMemoryStream.ToArray();
            }

            return data;
        }

        /// <summary>
        /// adds the signature image to the pdf
        /// </summary>
        /// <param name="PdfFile"></param>
        /// <param name="signatureBox"></param>
        /// <returns></returns>
        public byte[] AddImage(IFormFile PdfFile, SignatureBox signatureBox)
        {
            byte[] data;

            using (var pdfStream = PdfFile.OpenReadStream())
            using (var pdfMemoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(pdfMemoryStream);
                PdfReader reader = new PdfReader(pdfStream);
                PdfDocument pdfDoc = new PdfDocument(reader, writer);
                int pageCount = pdfDoc.GetNumberOfPages();

                Document doc = new Document(pdfDoc);

                ImageData imageData = ImageDataFactory.Create("resources/signature.jpg");
                Image image = new Image(imageData).ScaleAbsolute(height, height).SetFixedPosition(pageCount, signatureBox.xAxis, signatureBox.yAxis);
                doc.Add(image);

                doc.Close();
                pdfDoc.Close();
                data = pdfMemoryStream.ToArray();
            }

            return data;
        }

        /// <summary>
        /// testing signing with Syncfusion
        /// </summary>
        //public byte[] signPDFTemp(IFormFile pdfFile, IFormFile certificate)
        //{

        //    using (var pdfStream = pdfFile.OpenReadStream())
        //    using (var signedMemoryStream = new MemoryStream())
        //    {
        //        PdfReader reader = new PdfReader(pdfStream);
        //        PdfDocument pdf = new PdfDocument(reader);
        //        PdfLoadedDocument pdf3 = new PdfLoadedDocument(pdfStream);
        //        Syncfusion.Pdf.PdfDocument pdf2 = new Syncfusion.Pdf.PdfDocument();

        //        int startIndex = 0;
        //        int endIndex = pdf3.Pages.Count - 1;

        //        pdf2.ImportPageRange(pdf3, startIndex, endIndex);
        //        var page = pdf2.Pages[0];
        //        PdfGraphics graphics = page.Graphics;


        //        //Syncfusion.Pdf.PdfLoadedPage loadedPage = pdf3.Pages[0] as Syncfusion.Pdf.PdfLoadedPage;
        //        //pdf3.ImportPageRange(loadedDocument, startIndex, endIndex);
        //        pdf.GetPage(1);

        //        byte[] readText = File.ReadAllBytes(@"resources/ks2.pfx");

        //        System.Security.Cryptography.X509Certificates.X509Certificate2 temp = new System.Security.Cryptography.X509Certificates.X509Certificate2(readText, "password");

        //        PdfCertificate cert = new PdfCertificate(temp);

        //        Syncfusion.Pdf.Security.PdfSignature signature = new Syncfusion.Pdf.Security.PdfSignature(page, cert, "Signature");

        //        //PdfBitmap signatureImage = new PdfBitmap(@"resources/signature.png");
        //        //Image image1 = Image.FromFile("c:\\FakePhoto1.jpg");
        //        //PdfBitmap image = new PdfBitmap()

        //        using (var tream = new FileStream(@"resources/signature.png", FileMode.Open, FileAccess.Read))
        //        {
        //            PdfBitmap signatureImage = new PdfBitmap(tream);

        //            signature.Bounds = new Syncfusion.Drawing.RectangleF(0, 0, 200, 100);
        //            signature.ContactInfo = "johndoe@owned.us";
        //            signature.LocationInfo = "Honolulu, Hawaii";
        //            signature.Reason = "I am author of this document.";

        //            graphics.DrawImage(signatureImage, signature.Bounds);
        //        }

        //        MemoryStream stream = new MemoryStream();
        //        pdf2.Save(stream);
        //        pdf2.Close(true);

        //        return stream.ToArray();
        //    }
        //}
    }
}
