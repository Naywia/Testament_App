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

        private readonly PdfFont _boldFont;

        public PDFService()
        {
            // Load the default bold font
            //_boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.HELVETICA_BOLD);
            _boldFont = PdfFontFactory.CreateFont("Helvetica-Bold");
        }

        public byte[] GeneratePdf(string familyTreeJson, string password = "test")
        {
            var writerProperties = new WriterProperties()
                    .SetStandardEncryption(
                        userPassword: Encoding.UTF8.GetBytes(password),                // User password
                        ownerPassword: null,              // Owner password (has more permissions)
                        EncryptionConstants.ALLOW_PRINTING,        // Permissions (you can change these)
                        EncryptionConstants.ENCRYPTION_AES_128);
            using var stream = new MemoryStream();
            using (var writer = new PdfWriter(stream, writerProperties))
            {
                using var pdf = new PdfDocument(writer);
                // Add metadata
                pdf.GetDocumentInfo().SetTitle("Arvefordeleren");
                pdf.GetDocumentInfo().SetMoreInfo("FamilyTreeJSON", familyTreeJson);

                using var document = new Document(pdf);

                // Cover Page.
                CreateCoverPage(document, "~/images/background.png");
                AddCoverPage(document);

                // Family Tree Section.
                AddFamilyTree(document);

                // More Sections

            }

            return stream.ToArray();
        }

        private void AddCoverPage(Document document)
        {
            document.Add(new Paragraph("Arvefordeleren")
                .SetFontSize(24)
                .SetFont(_boldFont)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Lavet d. {DateTime.Now:dd MMMM yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE)); // Move to the next page
        }

        private void CreateCoverPage(Document document, string backgroundImagePath)
        {
            PageSize pageSize = PageSize.A4.Rotate();
            
            // Create a new page for the cover page
            var coverPage = document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            // Get the PdfCanvas object to draw on the cover page
            var canvas = new PdfCanvas(coverPage);

            // Set the background image (make sure the image is in the correct size or scale it)
            //var image = new Image(ImageDataFactory.Create(backgroundImagePath));

            // Scale the image to fit the page (if needed)
            //image.ScaleToFit(pdfDocument.GetDefaultPageSize().GetWidth(), pdfDocument.GetDefaultPageSize().GetHeight());

            // Set the image position to cover the entire page
            //image.SetFixedPosition(0, 0); // Start at the top-left corner
            canvas.AddImageFittedIntoRectangle(ImageDataFactory.Create(backgroundImagePath), pageSize, false);

            

            //// Add the blue overlay with transparency (mimicking the ::before effect in CSS)
            //canvas.SetFillColorRgb(0, 26, 51); // RGB color for the blue tint
            //canvas.SetFillOpacity(0.85f); // Set the opacity (85% opacity)
            //canvas.Rectangle(0, 0, pdfDocument.GetDefaultPageSize().GetWidth(), pdfDocument.GetDefaultPageSize().GetHeight());
            //canvas.Fill();

            canvas.SetFillColor(new DeviceRgb(0, 26, 51)); // RGB color for the blue tint

            canvas.SaveState();
            PdfExtGState state = new PdfExtGState().SetFillOpacity(0.85f);
            canvas.SetExtGState(state);

            // Set alpha (opacity) to 85% (0.85)
            //canvas.SetFillOpacity(0.85f); // This sets the opacity level for fill operations
            canvas.Rectangle(0, 0, pdfDocument.GetDefaultPageSize().GetWidth(), pdfDocument.GetDefaultPageSize().GetHeight());
            canvas.Fill();

            // Now add custom content over the background image and tint
            

            // Add Title Text
            var title = new Paragraph("Arvefordeleren")
                .SetFontSize(36)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(0, pdfDocument.GetDefaultPageSize().GetHeight() - 100, pdfDocument.GetDefaultPageSize().GetWidth());

            // Add subtitle or additional text
            var subtitle = new Paragraph("An in-depth report of your family history")
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(0, pdfDocument.GetDefaultPageSize().GetHeight() - 150, pdfDocument.GetDefaultPageSize().GetWidth());

            // Add content to the cover page
            document.Add(title);
            document.Add(subtitle);
        }


        private void AddFamilyTree(Document document)
        {
            document.Add(new Paragraph("Stamtræ")
                .SetFontSize(18)
                .SetFont(_boldFont));


            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE)); // Move to the next page
        }
    }
}