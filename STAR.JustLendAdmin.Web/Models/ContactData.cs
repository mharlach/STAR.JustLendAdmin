#nullable enable

using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class ContactData
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Ext")]
        public string PhoneExtension { get; set; } = string.Empty;
    }

}
