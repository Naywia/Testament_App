using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Text.Json;
using Testament_App.Models;

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

        public byte[] GeneratePdf(string familyTreeJson)
        {
            using var stream = new MemoryStream();
            using (var writer = new PdfWriter(stream))
            {
                using var pdf = new PdfDocument(writer);
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
            document.Add(new Paragraph("Arvefordeleren")
                .SetFontSize(24)
                .SetFont(_boldFont)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Lavet d. {DateTime.Now:dd MMMM yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE)); // Move to the next page
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