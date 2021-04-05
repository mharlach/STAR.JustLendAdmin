#nullable enable
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class VendorSystemData
    {
        [Required]
        [Display(Name = "Admin Id")]
        public string AdminId { get; set; } = string.Empty;

        [Display(Name = "Admin Password")]
        public string AdminPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "MOL Company Code")]
        public string MolCompanyCode { get; set; } = string.Empty;

        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
