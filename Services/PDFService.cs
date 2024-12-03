using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Text;

namespace Testament_App.Services
{
    public class PDFService
    { /// <summary>
      /// Generates a comprehensive FamilyTree PDF report.
      /// </summary>
      /// <param name="familyTreeJson">JSON representation of the family tree.</param>
      /// <returns>PDF file as a byte array.</returns>

        private readonly PdfFont _headingFont;
        private readonly PdfFont _bodyFont;

        public PDFService()
        {
            // Define font paths
            string merriweatherPath = System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot/fonts/Merriweather/Merriweather-Bold.ttf");
            string quicksandPath = System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot/fonts/Quicksand/static/Quicksand-Regular.ttf");

            // Ensure font files exist
            if (!File.Exists(merriweatherPath))
                throw new FileNotFoundException("Heading font file not found.", merriweatherPath);

            if (!File.Exists(quicksandPath))
                throw new FileNotFoundException("Body font file not found.", quicksandPath);

            // Load fonts
            _headingFont = PdfFontFactory.CreateFont(merriweatherPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            _bodyFont = PdfFontFactory.CreateFont(quicksandPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
        }

        public byte[] GeneratePdf(string familyTreeJson, string password = "test")
        {
            var writerProperties = new WriterProperties()
                    .SetStandardEncryption(
                        userPassword: Encoding.UTF8.GetBytes(password),
                        // Owner password is set to null (has more permissions)
                        ownerPassword: null,
                        // Permissions
                        EncryptionConstants.ALLOW_PRINTING,
                        EncryptionConstants.ENCRYPTION_AES_128);
            using var stream = new MemoryStream();
            using (var writer = new PdfWriter(stream, writerProperties))
            {
                using var pdf = CreateCoverPage(writer);
                // Add metadata
                pdf.GetDocumentInfo().SetTitle("Arvefordeleren");
                pdf.GetDocumentInfo().SetMoreInfo("FamilyTreeJSON", familyTreeJson);

                using var document = new Document(pdf);

                // Cover Page.
                AddCoverPage(document);

                // Family Tree Section.
                AddFamilyTree(document);

                // More Sections

            }

            return stream.ToArray();
        }

        private void AddCoverPage(Document document)
        {
            // Set the text color to white
            var whiteColor = ColorConstants.WHITE;

            document.Add(new Paragraph("Arvefordeleren")
                .SetFontSize(36)
                .SetFont(_headingFont)
                .SetFontColor(whiteColor) // Set the font color to white
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Lavet d. {DateTime.Now:dd MMMM yyyy}")
                .SetFontSize(24)
                .SetFont(_bodyFont)
                .SetFontColor(whiteColor) // Set the font color to white
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE)); // Move to the next page
        }

        private PdfDocument CreateCoverPage(PdfWriter writer)
        {
            // Set the page size to standard A4
            PageSize pageSize = PageSize.A4;

            var pdf = new PdfDocument(writer);

            // Create a new page with standard A4 dimensions
            var coverPage = pdf.AddNewPage(pageSize);

            // Get the PdfCanvas object to draw on the cover page
            var canvas = new PdfCanvas(coverPage);

            // Load the image
            string fullPath = System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot/images/background.png");
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Background image not found.", fullPath);
            }

            var imageData = ImageDataFactory.Create(fullPath);
            var image = new Image(imageData);

            // Get the dimensions of the page and the image
            float pageWidth = pageSize.GetWidth();
            float pageHeight = pageSize.GetHeight();
            float imageWidth = image.GetImageWidth();
            float imageHeight = image.GetImageHeight();

            // Calculate the scaling factor to fill the page while maintaining aspect ratio
            float widthScale = pageWidth / imageWidth;
            float heightScale = pageHeight / imageHeight;
            float scale = Math.Max(widthScale, heightScale);

            // Calculate the scaled image dimensions
            float scaledWidth = imageWidth * scale;
            float scaledHeight = imageHeight * scale;

            // Center the cropped image on the page
            float xOffset = (pageWidth - scaledWidth) / 2;
            float yOffset = (pageHeight - scaledHeight) / 2;

            // Draw the scaled and cropped image
            canvas.AddImageFittedIntoRectangle(imageData,
                new Rectangle(xOffset, yOffset, scaledWidth, scaledHeight), false);

            // Apply a blue overlay with transparency
            canvas.SetFillColor(new DeviceRgb(0, 26, 51));
            PdfExtGState state = new PdfExtGState().SetFillOpacity(0.85f);
            canvas.SaveState().SetExtGState(state);
            canvas.Rectangle(0, 0, pageWidth, pageHeight);
            canvas.Fill();
            canvas.RestoreState();

            return pdf;
        }



        private void AddFamilyTree(Document document)
        {
            document.Add(new Paragraph("Stamtræ")
                .SetFontSize(18)
                .SetFont(_headingFont));


            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE)); // Move to the next page
        }
    }
}