#nullable enable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class SystemData
    {
        [Required]
        [Display(Name = "Contact / Admin Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Document Email")]
        [DataType(DataType.EmailAddress)]
        public string DocumentEmail { get; set; } = string.Empty;
    }

}
