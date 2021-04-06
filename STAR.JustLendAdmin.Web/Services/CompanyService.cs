#nullable enable
using Microsoft.Extensions.Configuration;
using STAR.JustLendAdmin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STAR.JustLendAdmin.Web.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAsync();
        Task<Company?> GetAsync(string id);
        Task<UpsertModelResponse<Company>> UpsertAsync(Company company);
    }

    public class FakeCompanyService : ICompanyService
    {
        private Company[] companies = new[]
        {
            new Company{Id = "1", General = new GeneralData{Name = "Fool Co"}},
            new Company{Id = "2", General = new GeneralData{ Name = "Trash LLC"}},
        };


        public Task<IEnumerable<Company>> GetAsync()
        {
            return Task.FromResult<IEnumerable<Company>>(companies);
        }

        public Task<Company?> GetAsync(string id)
        {
            return Task.FromResult<Company?>(companies.FirstOrDefault(x => x.Id == id));
        }

        public Task<UpsertModelResponse<Company>> UpsertAsync(Company company)
        {
            if (string.IsNullOrEmpty(company.Id))
            {
                company.Id = Guid.NewGuid().ToString();
            }
            else
            {
                for (int i = 0; i < companies.Length; i++)
                {
                    if(companies[i].Id == company.Id)
                    {
                        companies[i] = company;
                        break;
                    }
                }
            }
            var response = new UpsertModelResponse<Company> { Success = true, Model = company };
            return Task.FromResult(response);
        }
    }

    
}
