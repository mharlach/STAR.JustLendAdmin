#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using STAR.JustLendAdmin.Web.Models;


namespace STAR.JustLendAdmin.Web.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAsync(UserSearchRequest request);

        Task<User?> GetAsync(string userId);

        Task<UpsertModelResponse<User>> UpsertAsync(User user);
    }

    public class UserService : DbContextBase, IUserService
    {
        private readonly ILogger<UserService> log;

        public UserService(IConfiguration configuration, ILogger<UserService> log)
            : base(configuration)
        {
            this.log = log;
        }

        public async Task<IEnumerable<User>> GetAsync(UserSearchRequest request)
        {
            if (request is UserSearchFilterRequest filterRequest)
            {
                return await GetFilteredUsersAsync(filterRequest);
            }
            else
            {
                return await GetAllCompanyUsersAsync(request);
            }

        }

        private async Task<IEnumerable<User>> GetFilteredUsersAsync(UserSearchFilterRequest filterRequest)
        {
            var users = new List<User>();
            using var conn = await base.GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,FirstName,LastName,Email," +
                "MOLID,Active,TeamCode,KY,LoanConsultant,TeamManager," +
                "ProcessorAssignmentOverride,ClosingCoordinator,TeamLead," +
                "LoanConsultant,ExecutiveManager,Underwriter,Funder,LoanSetup" +
                "FROM tblUsers WHERE " +
                "Active=@Active AND (" +
                "(TeamCode=@Filter) OR " +
                "(FirstName LIKE @LikeFilter) OR " +
                "(LastName LIKE @LikeFilter) OR " +
                "(MOLID=@Filter) OR " +
                "(Email LIKE @LikeFilter))";

            if (filterRequest.SupressTeam)
                cmd.CommandText += " AND TeamCode!='000' ";

            if (filterRequest.Agents)
                cmd.CommandText += " AND LoanConsultant=1 ";

            if (filterRequest.Processors)
                cmd.CommandText += " AND ClosingCoordinator=1 ";

            if (filterRequest.TeamManagers)
                cmd.CommandText += " AND TeamManager=1 ";

            if (filterRequest.ExecutiveManagers)
                cmd.CommandText += " AND ExecutiveManager=1 ";

            if (filterRequest.Underwriters)
                cmd.CommandText += " AND Underwriter=1 ";

            if (filterRequest.Funders)
                cmd.CommandText += " AND Funder=1 ";

            if (filterRequest.LoanSetup)
                cmd.CommandText += " AND LoanSetup=1 ";

            cmd.CommandText += " AND ISNULL(Locked,'')='0' AND CompanyId=1092";

            cmd.Parameters.Add(BuildParameter("@Active", filterRequest.Active));
            cmd.Parameters.Add(BuildParameter("@Filter", filterRequest.Filter));
            cmd.Parameters.Add(BuildParameter("@LikeFilter", $"%{filterRequest.Filter}%"));

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var user = GetUser(rdr);
                users.Add(user);
            }

            return users;
        }

        private async Task<IEnumerable<User>> GetAllCompanyUsersAsync(UserSearchRequest request)
        {
            var users = new List<User>();
            using var conn = await base.GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_UserList";
            cmd.Parameters.Add(BuildParameter("@CompanyId", request.CompanyId));
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var user = GetUser(rdr);
                users.Add(user);
            }

            return users;
        }

        public async Task<User?> GetAsync(string userId)
        {
            using var conn = await GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID, Active, CompanyID, FirstName, LastName, Email, Phone" +
                                      ", TeamCode, MOLID, LoanConsultant, ClosingCoordinator, TeamLead, TeamManager" +
                                      ", ProcessorAssignmentOverride, AgentProcessor, Underwriter, Funder, LoanSetup" +
                                      ", GlobalManager, RegionalManager, ExecutiveManager, TitleVendorOverride, KY" +
                                      " FROM tblUser WHERE ID=@ID";
            cmd.Parameters.Add(BuildParameter("@ID", userId));

            var rdr = cmd.ExecuteReader();
            if (rdr.Read() == false)
            {
                return null;
            }

            var user = GetUser(rdr);
            return user;

        }

        private User GetUser(IDataReader rdr)
        {
            var user = new User
            {
                Id = (string)rdr["ID"],
                CompanyId = (string)rdr["CompanyId"],
                FirstName = (string)rdr["FirstName"],
                LastName = (string)rdr["LastName"],
                Email = (string)rdr["Email"],
                PhoneNumber = (string)rdr["Phone"],
                TeamCode = (string)rdr["TeamCode"],
                EncompassId = (string)rdr["MOLID"],
                Agent = (bool)rdr["LoanConsultant"],
                Processor = (bool)rdr["ClosingCoordinator"],
                ExecutiveManager = (bool)rdr["ExecutiveManager"],
                Underwriter = (bool)rdr["Underwriter"],
                Funder = (bool)rdr["Funder"],
                Active = (bool)rdr["Active"],
                GlobalManager = (bool)rdr["GlobalManager"],
                AgentProcessor = (bool)rdr["AgentProcessor"],
                TitleVendorOverride = (bool)rdr["TitleVendorOverride"],
                ProcessorAssignmentOverride = (bool)rdr["ProcessorAssignmentOverride"],
                KyProcessor = rdr["KY"] as bool? ?? false,
                TeamManager = (bool)rdr["TeamManager"],
                TeamLead = rdr["TeamLead"] as bool? ?? false,
            };
            var loanSetup = rdr["LoanSetup"] as string;
            if (loanSetup == "1")
            {
                user.LoanSetUp = true;
            }

            if (user.Agent && user.TeamManager && user.ProcessorAssignmentOverride)
            {
                user.Agent = false;
                user.AgentTeamManager = true;
            }
            else if (user.TeamManager && user.Processor && user.ProcessorAssignmentOverride)
            {
                user.Processor = false;
                user.ProcessorTeamManager = true;
            }

            var teamLead = rdr["TeamLead"] as bool? ?? false;
            if (teamLead && user.Processor && user.ProcessorAssignmentOverride)
            {
                user.ProcessorTeamLead = true;
                user.Processor = false;
            }

            return user;
        }

        public async Task<UpsertModelResponse<User>> UpsertAsync(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Id))
                {
                    await InsertAsync(user);
                }
                else
                {
                    await UpdateAsync(user);
                }

                return new UpsertModelResponse<User>
                {
                    Model = user,
                    Message = "User upsert successful",
                    Success = true,
                };

            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error upserting user model - {JsonConvert.SerializeObject(user)}");
                return new UpsertModelResponse<User>
                {
                    Model = user,
                    Message = "An error occurred during upsert",
                    Success = false,
                };
            }
        }

        private async Task InsertAsync(User user)
        {
            if (user.AgentTeamManager == false || user.ProcessorTeamManager == false || user.ProcessorTeamLead == false)
            {
                user.ProcessorAssignmentOverride = false;
            }
            if (user.Active && user.AgentTeamManager)
            {
                user.ProcessorAssignmentOverride = true;
                user.Agent = true;
            }
            if (user.Active && (user.ProcessorTeamManager || user.ProcessorTeamLead))
            {
                user.Processor = false;
                user.ProcessorAssignmentOverride = true;
            }

            if (user.Underwriter || user.Funder || user.ExecutiveManager)
            {
                user.TeamCode = "000";
            }

            using var conn = await GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "sp_UserInsert";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(BuildParameter("@Created_User", "TODO"));
            cmd.Parameters.Add(BuildParameter("@CompanyId", user.CompanyId));
            cmd.Parameters.Add(BuildParameter("@FirstName", user.FirstName));
            cmd.Parameters.Add(BuildParameter("@LastName", user.LastName));
            cmd.Parameters.Add(BuildParameter("@Email", user.Email));
            cmd.Parameters.Add(BuildParameter("@Phone", user.PhoneNumber));
            cmd.Parameters.Add(BuildParameter("@Password", ""));
            cmd.Parameters.Add(BuildParameter("@Active", user.Active));
            cmd.Parameters.Add(BuildParameter("@LoanConsultant", user.Agent));
            cmd.Parameters.Add(BuildParameter("@ClosingCoordinator", user.Processor));
            cmd.Parameters.Add(BuildParameter("@AgentProcessor", user.AgentProcessor));
            cmd.Parameters.Add(BuildParameter("@Underwriter", user.Underwriter));
            cmd.Parameters.Add(BuildParameter("@Funder", user.Funder));
            if (user.AgentTeamManager && user.ProcessorTeamManager)
            {
                cmd.Parameters.Add(BuildParameter("@TeamManager", true));
            }
            if (user.ProcessorTeamLead)
            {
                cmd.Parameters.Add(BuildParameter("@TeamLead", user.ProcessorTeamLead));
            }
            cmd.Parameters.Add(BuildParameter("@RegionalManager", false));
            cmd.Parameters.Add(BuildParameter("@GlobalManager", user.GlobalManager));
            cmd.Parameters.Add(BuildParameter("@ExecutiveManager", user.ExecutiveManager));

            cmd.Parameters.Add(BuildParameter("@ProcessorAssignmentOverride", user.ProcessorAssignmentOverride));
            cmd.Parameters.Add(BuildParameter("@TitleVendorOverride", user.TitleVendorOverride));
            cmd.Parameters.Add(BuildParameter("@KYProcessor", user.KyProcessor));
            cmd.Parameters.Add(BuildParameter("@LoanSetup", user.LoanSetUp ? 1 : 0));
            cmd.Parameters.Add(BuildParameter("@TeamCode", user.TeamCode));
            if (string.IsNullOrWhiteSpace(user.EncompassId) == false)
            {
                cmd.Parameters.Add(BuildParameter("@MOLID", user.EncompassId));
            }

            cmd.ExecuteNonQuery();
        }

        private async Task UpdateAsync(User user)
        {
            if (user.AgentTeamManager == false || user.ProcessorTeamManager == false || user.ProcessorTeamLead == false)
            {
                user.ProcessorAssignmentOverride = false;
            }
            if (user.Active && user.AgentTeamManager)
            {
                user.ProcessorAssignmentOverride = true;
                user.Agent = true;
            }
            if (user.Active && (user.ProcessorTeamManager || user.ProcessorTeamLead))
            {
                user.Processor = false;
                user.ProcessorAssignmentOverride = true;
            }

            if (user.Underwriter || user.Funder || user.ExecutiveManager)
            {
                user.TeamCode = "000";
            }

            using var conn = await GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "sp_UserUpdate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(BuildParameter("@ID", user.Id));
            cmd.Parameters.Add(BuildParameter("@Created_User", "TODO"));
            cmd.Parameters.Add(BuildParameter("@CompanyId", user.CompanyId));
            cmd.Parameters.Add(BuildParameter("@FirstName", user.FirstName));
            cmd.Parameters.Add(BuildParameter("@LastName", user.LastName));
            if (user.Active)
            {
                cmd.Parameters.Add(BuildParameter("@Email", user.Email));
            }
            else
            {
                cmd.Parameters.Add(BuildParameter("@Email", string.Empty));
            }

            cmd.Parameters.Add(BuildParameter("@Phone", user.PhoneNumber));
            cmd.Parameters.Add(BuildParameter("@Password", ""));
            cmd.Parameters.Add(BuildParameter("@Active", user.Active));
            cmd.Parameters.Add(BuildParameter("@LoanConsultant", user.Agent));
            cmd.Parameters.Add(BuildParameter("@ClosingCoordinator", user.Processor));
            cmd.Parameters.Add(BuildParameter("@AgentProcessor", user.AgentProcessor));
            cmd.Parameters.Add(BuildParameter("@Underwriter", user.Underwriter));
            cmd.Parameters.Add(BuildParameter("@Funder", user.Funder));
            if (user.AgentTeamManager && user.ProcessorTeamManager)
            {
                cmd.Parameters.Add(BuildParameter("@TeamManager", true));
            }
            if (user.ProcessorTeamLead)
            {
                cmd.Parameters.Add(BuildParameter("@TeamLead", user.ProcessorTeamLead));
            }
            cmd.Parameters.Add(BuildParameter("@RegionalManager", false));
            cmd.Parameters.Add(BuildParameter("@GlobalManager", user.GlobalManager));
            cmd.Parameters.Add(BuildParameter("@ExecutiveManager", user.ExecutiveManager));

            cmd.Parameters.Add(BuildParameter("@ProcessorAssignmentOverride", user.ProcessorAssignmentOverride));
            cmd.Parameters.Add(BuildParameter("@TitleVendorOverride", user.TitleVendorOverride));
            cmd.Parameters.Add(BuildParameter("@KYProcessor", user.KyProcessor));
            cmd.Parameters.Add(BuildParameter("@LoanSetup", user.LoanSetUp ? 1 : 0));
            cmd.Parameters.Add(BuildParameter("@TeamCode", user.TeamCode));
            if (string.IsNullOrWhiteSpace(user.EncompassId) == false)
            {
                cmd.Parameters.Add(BuildParameter("@MOLID", user.EncompassId));
            }

            cmd.ExecuteNonQuery();
        }


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
            if (user.Id.Length == 0)
            {
                user.Id = (users.Select(x => int.Parse(x.Id)).Max() + 1).ToString();
            }
            users.Add(user);
            var response = new UpsertModelResponse<User> { Success = true, Model = user };
            return Task.FromResult(response);
        }
    }
}