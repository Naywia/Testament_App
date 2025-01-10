using System.ComponentModel.DataAnnotations;

namespace Testament_App.Models
{
    public class Contact
    {
        [Required]
        [StringLength(100)]
        public string Message { get; set; }
    }
}
