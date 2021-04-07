using System;
using System.ComponentModel.DataAnnotations;

namespace STAR.JustLendAdmin.Web.Models
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string CompanyId { get; set; }

        [Required]
        [Display(Name = "First Name")]
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

        [Display(Name = "Agent Team Manager")]
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
        public bool GlobalManager { get; set; }
        public bool AgentProcessor { get; set; }
        public bool TitleVendorOverride { get; set; }
        public bool ProcessorAssignmentOverride { get; set; }
        public bool TeamManager { get; set; }
        public bool TeamLead { get; set; }

        public string DetermineRole()
        {
            if (Agent && TeamManager && ProcessorAssignmentOverride)
            {
                return "Agent Team Manager";
            }
            else if (Processor && TeamManager && ProcessorAssignmentOverride)
            {
                return "Processor Team Manager";
            }
            else if (Processor && TeamLead && ProcessorAssignmentOverride)
            {
                return "Processor Team Lead";
            }
            else if (KyProcessor)
            {
                return "KY Processor";
            }
            else if (Agent)
            {
                return "Agent";
            }
            else if (Processor)
            {
                return "Processor";
            }
            else if (ExecutiveManager)
            {
                return "Executive Manager";
            }
            else if (Underwriter)
            {
                return "Underwriter";
            }
            else if (Funder)
            {
                return "Funder";
            }
            else if (LoanSetUp)
            {
                return "LoanSetup";
            }
            else
            {
                return "Undefined";
            }
        }
    }
}