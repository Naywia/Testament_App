namespace Testament_App.Models
{
    public class PdfImport
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public PdfImport(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}
