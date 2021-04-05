#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using STAR.JustLendAdmin.Web.Models;


namespace STAR.JustLendAdmin.Web.Services
{
    public interface IUserService {
        Task<IEnumerable<User>> GetAsync(UserSearchRequest request);

        Task<User?> GetAsync(string userId);

        Task<UpsertModelResponse<User>> UpsertAsync(User user);
    }


    public class FakeUserService : IUserService
    {
        private static List<User> users = new List<User>
        {
            new User{Id = "1", FirstName = "Larry", LastName = "Stooge"},
            new User{Id = "2", FirstName = "Moe", LastName = "Stooge"},
            new User{Id = "3", FirstName = "Curly", LastName = "Stooge"},
        };

        public Task<IEnumerable<User>> GetAsync(UserSearchRequest request)
        {
            return Task.FromResult<IEnumerable<User>>(users);
        }

        public Task<User?> GetAsync(string userId)
        {
            return Task.FromResult<User?>(users.FirstOrDefault(x => x.Id == userId));
        }

        public Task<UpsertModelResponse<User>> UpsertAsync(User user)
        {
            if(user.Id.Length == 0)
            {
                user.Id = (users.Select(x =>int.Parse(x.Id)).Max() + 1).ToString();
            }
            users.Add(user);
            var response = new UpsertModelResponse<User> { Success = true, Model = user };
            return Task.FromResult(response);
        }
    }

    public class UserSearchRequest
    {
        public int CompanyId { get; set; } 
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