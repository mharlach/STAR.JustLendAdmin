#nullable enable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public enum VendorType
    {
        [Display(Name = "4506")]
        [Description("4506")]
        FourFiveZeroSix,
        Signing,
        Flood,
        Appraisal,
        Title,
        Escrow,
        Realtor
    }
}
