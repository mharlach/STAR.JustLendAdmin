#nullable enable

using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class GeneralData
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; } = string.Empty;

        [Display(Name = "Address 2")]
        public string? Address2 { get; set; } 

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string Zip { get; set; } = string.Empty;

        [Display(Name = "Company Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; } 

        [Display(Name = "Company Fax Number")]
        [DataType(DataType.PhoneNumber)]
        public string? FaxNumber { get; set; } 
    }

}
