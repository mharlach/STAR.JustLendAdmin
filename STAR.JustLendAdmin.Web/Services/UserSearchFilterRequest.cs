#nullable enable


namespace STAR.JustLendAdmin.Web.Services
{
    public class UserSearchFilterRequest : UserSearchRequest
    {
        public string Filter { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Agents { get; set; }
        public bool Processors { get; set; }
        public bool TeamLeads { get; set; }
        public bool TeamManagers { get; set; }
        public bool SupressTeam { get; set; }
        public bool Underwriters { get; set; }
        public bool Funders { get; set; }
        public bool LoanSetup { get; set; }
        public bool ExecutiveManagers { get; set; }
    }
}