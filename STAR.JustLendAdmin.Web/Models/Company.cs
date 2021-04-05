#nullable enable

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STAR.JustLendAdmin.Web.Models
{
    public class Company
    {
        public string Id { get; set; } = string.Empty;

        public GeneralData General { get; set; } = new GeneralData();
        public ContactData Contact { get; set; } = new ContactData();
        public SystemData System { get; set; } = new SystemData();
        public CompanyServiceInformation ServiceInformation { get; set; } = new CompanyServiceInformation();
        public CompanyDeposit Deposit { get; set; } = new CompanyDeposit();
        public CompanyNotification Notication { get; set; } = new CompanyNotification();
    }

    public class UpsertModelResponse<T> : ResponseCore 
        where T : class
    {
        public T? Model { get; set; }
    }

    public class CompanyViewModel
    {
        public Company Company { get; set; } = new Company();
        public ResponseCore Response { get; set; } = new ResponseCore();
    }
}
