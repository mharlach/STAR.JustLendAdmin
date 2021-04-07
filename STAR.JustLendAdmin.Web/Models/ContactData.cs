#nullable enable

using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class ContactData
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; } 

        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [Display(Name = "Ext")]
        public string? PhoneExtension { get; set; }
    }

}
