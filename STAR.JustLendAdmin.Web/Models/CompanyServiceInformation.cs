#nullable enable

using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class CompanyServiceInformation
    {
        [Display(Name = "4506")]
        public bool Requires4506 { get; set; }

        [Display(Name = "Signing")]
        public bool RequiresSigning { get; set; }

        [Display(Name = "Flood")]
        public bool RequiresFlood { get; set; }

        [Display(Name = "Apprasial")]
        public bool RequiresApprasial { get; set; }

        [Display(Name = "Title")]
        public bool RequiresTitle { get; set; }
        public Pricing Price4506 { get; set; } = new Pricing();
        public CompanySigningPrices SigningPrices { get; set; } = new CompanySigningPrices();
        public Pricing FloodPrices { get; set; } = new Pricing();
        public Pricing ApprasialPrices { get; set; } = new Pricing();
        public Pricing TitlePrices { get; set; } = new Pricing();

    }

}
