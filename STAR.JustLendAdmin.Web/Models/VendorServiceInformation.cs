#nullable enable
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class VendorServiceInformation
    {
        [Display(Name = "Standard Price")]
        public Pricing Pricing { get; set; } = new Pricing();
    }
}
