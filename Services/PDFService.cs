using iText.Html2pdf;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using System.Text;
using Testament_App.Models;

namespace Testament_App.Services
{
    public class PDFService
    { 
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

        #region Export

        public byte[] GeneratePdf(string password = "test")
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
                pdf.GetDocumentInfo().SetMoreInfo("FamilyTreeInfo", GetFamilyTreeJSON());

                using var document = new Document(pdf);

                // Cover Page.
                AddTextToCoverPage(document);

                // Family Tree Section.
                AddFamilyTree(document);

                // More Sections

            }

            return stream.ToArray();
        }

        private static PdfDocument CreateCoverPage(PdfWriter writer)
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

        private void AddTextToCoverPage(Document document)
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

        private static string GetFamilyTreeJSON()
        {
            List<Testator> testators = [.. Inheritance.GetTestators()];
            List<Person> heirs = [.. Inheritance.GetHeirs()];

            var familyTree = new
            {
                Testators = testators,
                Heirs = heirs
            };

            string json = JsonConvert.SerializeObject(familyTree, Formatting.Indented);

            Console.WriteLine(json);

            return json;
        }

        private void AddFamilyTree(Document document)
        {

            document.Add(new Paragraph("Stamtræ")
            .SetFontSize(18)
                .SetFont(_headingFont));

            try
            {
                // Fetch html
                //var htmlContent = FetchWebPageContent("https://localhost:7032/familytree").Result;
                var htmlContent = @"<html><head>
                    <style>
                        body {
                            margin: auto;
                            justify-content: center;
                            background-color: lightgreen;
                        }
                        .familyTreePerson {
                            width: 280px;
                            border: 1px solid black;
                            border-radius: 10px;
                            padding: 5px;
                            margin: 5px;
                        }
                        #familyTreeTestatorsContainer {
                            margin: auto;
                            width: 80%;
                            background-color: orange;
                            padding: 5px;
                        }
                        #familyTreeChildrenContainer {
                            margin: auto;
                            width: 80%;
                            background-color: lightblue;
                            padding: 5px;
                        }
                    </style>
                </head><body>";
                htmlContent += GenerateHtmlFamilyTree();

                htmlContent += "</body></html>";

                // Convert the web page content to a PDF and write to the MemoryStream
                ConverterProperties converterProperties = new();

                IList<IElement> elements = HtmlConverter.ConvertToElements(htmlContent, converterProperties);

                foreach (IElement element in elements)
                {
                    if (element is IBlockElement blockElement)
                    {
                        document.Add(blockElement);
                    }
                }
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        private static string GenerateHtmlFamilyTree()
        {
            // Recursive function to build HTML for a person and their descendants.
            string BuildDescendantHtml(Person person)
            {
                string html = $@"<div class=""familyTreePerson testator border border-dark border-1 rounded-3"">
            Name: {person.Name}
        </div>";

                // Handle children (if any).
                if (person.Children.Count > 0)
                {
                    html += @"<div class=""familyTreeChildren"">";
                    foreach (var child in person.Children)
                    {
                        html += BuildDescendantHtml(child);
                    }
                    html += "</div>";
                }

                return html;
            }

            // Helper function to generate ancestor HTML for a single person.
            string BuildAncestorHtml(Person person, int generationLevel)
            {
                string generationClass = generationLevel == 1 ? "parent" : "grandparent";
                string html = $@"<div class=""familyTreePerson {generationClass} border border-dark border-1 rounded-3"">
            Name: {person.Name}
        </div>";
                return html;
            }

            // Main HTML container for the family tree.
            string familyTreeHtml = @"<div id=""familyTreeContainer"">";

            // Fetch the testator(s) (only once).
            var testators = Inheritance.GetTestators();
            foreach (var testator in testators)
            {
                familyTreeHtml += @"<div id=""familyTreeTestatorsContainer""><h3>Testator</h3>";

                // Build ancestors manually from the testator's data.
                familyTreeHtml += @"<div id=""familyTreeParentsContainer""><h4>Parents & Grandparents</h4>";

                // Assume the testator object has properties `Parents` and `GrandParents`.
                if (testator.Parents != null)
                {
                    foreach (var parent in testator.Parents)
                    {
                        familyTreeHtml += BuildAncestorHtml(parent, 1); // Generation level 1 = parents.

                        // Build grandparents.
                        if (parent.Parents != null)
                        {
                            foreach (var grandparent in parent.Parents)
                            {
                                familyTreeHtml += BuildAncestorHtml(grandparent, 2); // Generation level 2 = grandparents.
                            }
                        }
                    }
                }
                familyTreeHtml += "</div>"; // Close Parents & Grandparents container.

                // Build descendants.
                familyTreeHtml += @"<div id=""familyTreeDescendantsContainer""><h4>Descendants</h4>";
                familyTreeHtml += BuildDescendantHtml(testator);
                familyTreeHtml += "</div></div>";
            }

            familyTreeHtml += "</div>"; // Close the main container.
            return familyTreeHtml;
        }

        #endregion

        #region Import

        public PdfImport ImportPdf(byte[] pdfContent, string password)
        {
            try
            {
                // Convert the password to a byte array and add it to ReaderProperties
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var readerProperties = new ReaderProperties().SetPassword(passwordBytes);

                // Open the PDF and validate the contents
                using var reader = new PdfReader(new MemoryStream(pdfContent), readerProperties);
                using var pdfDocument = new PdfDocument(reader);
                                
                if (pdfDocument.GetNumberOfPages() > 0)
                {
                    var info = pdfDocument.GetDocumentInfo();

                    // Get info for the family tree
                    string familyTreeInfo = info.GetMoreInfo("FamilyTreeInfo");
                    if (string.IsNullOrEmpty(familyTreeInfo))
                    {
                        Console.WriteLine("No FamilyTreeInfo metadata found.");
                    }
                    else
                    {
                        //Console.WriteLine($"{familyTreeInfo}");

                        ImportService.ImportFamilyTreeFromJson(familyTreeInfo);
                    }

                    return new(true, string.Empty);
                }
                else
                {
                    return new(false, "PDF'en indeholder ingen sider.");
                }
            }
            catch (BadPasswordException)
            {
                return new(false, "Forkert adgangskode. Prøv igen.");
            }
            catch (Exception ex)
            {
                return new(false, $"Fejl ved behandling af PDF: {ex.Message}");
            }
        }
        #endregion
    }
}