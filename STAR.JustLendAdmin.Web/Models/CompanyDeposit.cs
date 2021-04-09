#nullable enable

using System;
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class CompanyDeposit
    {
        public bool Required { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Only positive numbers allowed")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }

}
