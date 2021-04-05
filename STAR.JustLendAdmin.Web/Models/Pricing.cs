#nullable enable

using System;
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class Pricing
    {
        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }

}
