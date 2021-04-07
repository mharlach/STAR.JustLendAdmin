#nullable enable
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class VendorContact
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; } 

        public string? PhoneExtention { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; } 
    }
}
