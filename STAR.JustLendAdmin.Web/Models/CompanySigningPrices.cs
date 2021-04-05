#nullable enable

using System;
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class CompanySigningPrices : Pricing
    {
        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Display(Name = "E-Doc Price")]
        [DataType(DataType.Currency)]
        public decimal EDocPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Display(Name = "Concurrent 2nd")]
        [DataType(DataType.Currency)]
        public decimal Concurrent2nd { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Display(Name = "Attorney Closing")]
        [DataType(DataType.Currency)]
        public decimal AttorneyClosing { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Display(Name = "Single Signing")]
        [DataType(DataType.Currency)]
        public decimal SingleSigning { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Display(Name = "Refused Signing")]
        [DataType(DataType.Currency)]
        public decimal RefusedSigning { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [Display(Name = "No-Show")]
        [DataType(DataType.Currency)]
        public decimal NoShow { get; set; }
    }

}
