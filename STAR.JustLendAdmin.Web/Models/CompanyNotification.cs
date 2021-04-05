#nullable enable

using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class CompanyNotification
    {
        [Display(Name = "Cancel Orders - Notify Processor")]
        public bool NotifyProcessor { get; set; }

        [Display(Name = "Cancel Orders - Notify Agent / Sales Rep")]
        public bool NotifyAgentRep { get; set; }
    }

}
