# PDF Manager API Documentation

The PDF Manager API is a .NET-based service that allows you to sign PDF documents using certificates. This API provides a simple and efficient way to sign PDF files programmatically.

## Endpoints

### Sign PDF

Endpoint to sign a provided PDF document with a provided certificate.

**URL**: `/Pdf/sign-pdf`

**Method**: POST

**Parameters**:

| Name          | Type          | Description                                          |
|---------------|---------------|------------------------------------------------------|
| pdfFile       | File          | The PDF file to be signed.                          |
| certificate   | File          | The certificate file to be used for signing.        |
| password      | String        | The password for the certificate, if required.      |
| author        | Boolean       | Specifies whether the signer is the author.         |

**Response**:

- HTTP Status: 200 OK
- Content-Type: `application/pdf`
- Content-Disposition: `attachment; filename="signed.pdf"`

**Example Request**:

```http
POST /PdfController/sign-pdf HTTP/1.1
Content-Type: multipart/form-data; boundary=--------------------------1234567890
Host: your-api-domain.com

Content-Disposition: form-data; name="pdfFile"; filename="example.pdf"
Content-Type: application/pdf

<binary-data-of-pdf-file>

Content-Disposition: form-data; name="certificate"; filename="certificate.pfx"
Content-Type: application/octet-stream

<binary-data-of-certificate>

Content-Disposition: form-data; name="password"
Content-Type: text/plain

your-certificate-password

Content-Disposition: form-data; name="author"
Content-Type: text/plain

true
```

### Convert PDF to PDF/A3 Endpoint

The "convert-to-PdfA" endpoint of the PDF Manager API allows you to convert a received PDF document to the PDF/A3 standard. PDF/A3 is an archival format for long-term preservation of electronic documents. This endpoint ensures that the converted PDF complies with the PDF/A3 standard, making it suitable for archiving and long-term storage.

**URL**: `/Pdf/convert-to-PdfA`

**Method**: POST

**Parameters**:

| Name      | Type          | Description                                  |
|-----------|---------------|----------------------------------------------|
| pdfFile   | File          | The PDF file to be converted to PDF/A3.     |

**Response**:

- HTTP Status: 200 OK
- Content-Type: `application/pdf`
- Content-Disposition: `attachment; filename="converted.pdf"`

**Example Request**:

```http
POST /PdfController/convert-to-PdfA HTTP/1.1
Content-Type: multipart/form-data; boundary=--------------------------1234567890
Host: your-api-domain.com

Content-Disposition: form-data; name="pdfFile"; filename="example.pdf"
Content-Type: application/pdf

<binary-data-of-pdf-file>
```


## getting started

- you need to have .NET installed on your machine
- clone the project on your computer
- open the command line in the projct directory
- build the projct to insure dependencies -> dotnet build
- run the project -> dotnet run
- after running the project, the command line will show the port the project is running on
- the API should now be accessible at http://localhost:5030/swagger
