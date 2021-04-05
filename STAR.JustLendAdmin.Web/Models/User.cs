using System;
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Team Code")]
        public string TeamCode { get; set; }

        [Required]
        [Display(Name = "Encompass Id")]
        public string EncompassId { get; set; }

        public bool Agent { get; set; }

        [Display(Name ="Agent Team Manager")]
        public bool AgentTeamManager { get; set; }

        public bool Processor { get; set; }

        [Display(Name = "Processor Team Lead")]
        public bool ProcessorTeamLead { get; set; }

        [Display(Name = "Processor Team Manager")]
        public bool ProcessorTeamManager { get; set; }

        public bool Underwriter { get; set; } 

        public bool Funder { get; set; }

        [Display(Name = "Executive Manager")]
        public bool ExecutiveManager { get; set; }

        [Display(Name = "Loan Setup")]
        public bool LoanSetUp { get; set; }

        [Display(Name = "KY Processor")]
        public bool KyProcessor { get; set; }

        public bool Active { get; set; } = true;
    }
}