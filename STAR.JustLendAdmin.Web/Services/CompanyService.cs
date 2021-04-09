#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    public class CompanyService : DbContextBase, ICompanyService
    {
        private readonly ILogger<CompanyService> log;

        public CompanyService(ILogger<CompanyService> log, IConfiguration configuration) : base(configuration)
        {
            this.log = log;
        }

        public async Task<IEnumerable<Company>> GetAsync()
        {
            var companies = new List<Company>();

            using var conn = await GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM tblCompany";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var c = Read(rdr);
                companies.Add(c);
            }

            return companies;
        }

        private Company Read(System.Data.IDataReader rdr)
        {
            var company = new Company
            {
                Id = (int)rdr["ID"],
                General = new GeneralData
                {
                    Name = rdr["CompanyName"] as string ?? string.Empty,
                    Address1 = rdr["Address"] as string ?? string.Empty,
                    Address2 = rdr["Address2"] as string ?? string.Empty,
                    City = rdr["City"] as string ?? string.Empty,
                    Zip = rdr["Zip"] as string ?? string.Empty,
                },
                Contact = new ContactData
                {
                    FirstName = rdr["ContactFirstName"] as string ?? string.Empty,
                    LastName = rdr["ContactLastName"] as string ?? string.Empty,
                    Phone = rdr["ContactPhone"] as string ?? string.Empty,
                    PhoneExtension = rdr["ContactExt"] as string ?? string.Empty,
                },
                System = new SystemData
                {
                    Active = (bool)rdr["Active"],
                    DocumentEmail = rdr["DocumentEmail"] as string ?? string.Empty,
                },
                ServiceInformation = new CompanyServiceInformation
                {
                    Requires4506 = rdr["Need4506"] as bool? ?? false,
                    RequiresSigning = rdr["NeedSigning"] as bool? ?? false,
                    RequiresApprasial = rdr["NeedAppraisal"] as bool?  ?? false,
                    RequiresFlood = rdr["NeedFlood"] as bool? ?? false,
                    RequiresTitle = rdr["NeedTitle"] as bool? ?? false,
                    Price4506 = new Pricing
                    {
                        Price = (decimal)rdr["Price4506"],
                    },
                    SigningPrices = new CompanySigningPrices
                    {
                        Price = (decimal)rdr["PriceSigning"],
                        EDocPrice = (decimal)rdr["SEDoc"],
                        Concurrent2nd = (decimal)rdr["SConcurrent2nd"],
                        AttorneyClosing = (decimal)rdr["SAttorneyClosing"],
                        SingleSigning = (decimal)rdr["SSingleSigning"],
                        RefusedSigning = (decimal)rdr["SRefusedSigning"],
                        NoShow = (decimal)rdr["SNoShow"],
                    },
                    ApprasialPrices = new Pricing
                    {
                        Price = (decimal)rdr["PriceAppraisal"],
                    },
                    FloodPrices = new Pricing
                    {
                        Price = (decimal)rdr["PriceFlood"],
                    },
                    TitlePrices = new Pricing
                    {
                        Price = (decimal)rdr["PriceTitle"],
                    },
                },
                Deposit = new CompanyDeposit
                {
                    Required = rdr["DepositRequired"] as bool? ?? false,
                    Amount = rdr["DepositAmt"] as decimal? ?? 0,
                },
                Notication = new CompanyNotification
                {
                    NotifyAgentRep = rdr["CancelOrdersNotifySalesRep"] as bool? ?? false,
                    NotifyProcessor = rdr["CancelOrdersNotifyProcessor"] as bool? ?? false,
                },
            };

            return company;
        }

        public async Task<Company?> GetAsync(int id)
        {
            using var conn = await GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM tblCompany WHERE ID=@Id";
            cmd.Parameters.Add(BuildParameter("@Id", id));
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return Read(rdr);
            }

            return null;

        }

        public async Task<UpsertModelResponse<Company>> UpsertAsync(Company company)
        {
            if(company.Id  == 0)
            {
                return await InsertAsync(company);
            }
            else
            {
                return await UpdateAsync(company);
            }
        }

        public async Task<UpsertModelResponse<Company>> UpdateAsync(Company c)
        {
            using var conn = await GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE tblCompany SET " +
                "CompanyName=@CompanyName,Address=@Address,Address2=@Address2,City=@City,State=@State,Zip=@Zip,Phone=@Phone,Fax=@Fax" +
                "ContactFirstName=@ContactFirstName,ContactLastName=@ContactLastName,ContactPhone=@ContactPhone,ContactExt=@ContactExt," +
                "Active=@Active," +
                "Need4506=@Need4506,NeedSigning=@NeedSigning,NeedAppraisal=@NeedAppraisal,NeedFlood=@NeedFlood,NeedTitle=@NeedTitle," +
                "Price4506=@Price4506," +
                "PriceSigning=@PriceSigning,SEDoc=@SEDoc,SConcurrent2nd=@SConcurrent2nd,SAttorneyClosing=@SAttorneyClosing,SSingleSigning=@SSigningSigning,SRefusedSigning=@SRefusedSigning,SNoShow=@SNoShow," +
                "PriceAppraisal=@PriceAppraisal,PriceFlood=@PriceFlood,PriceTitle=@PriceTitle," +
                "DepositRequired=@DepositRequired,DepositAmt=@DepositAmt," +
                "CancelOrdersNotifyProcessor=@CancelOrdersNotifyProcessor,CancelOrdersNotifySalesRep=@CancelOrdersNotifySalesRep," +
                "DocumentEmail=@DocumentEmail" +
                "WHERE" +
                "ID=@ID";

            cmd.Parameters.Add(BuildParameter("@ID", c.Id));
            cmd.Parameters.Add(BuildParameter("@CompanyName", c.General.Name));
            cmd.Parameters.Add(BuildParameter("@Address", c.General.Address1));
            cmd.Parameters.Add(BuildParameter("@Address2", c.General.Address2));
            cmd.Parameters.Add(BuildParameter("@City", c.General.City));
            cmd.Parameters.Add(BuildParameter("@State", c.General.State));
            cmd.Parameters.Add(BuildParameter("@Zip", c.General.Zip));
            cmd.Parameters.Add(BuildParameter("@Phone", c.General.PhoneNumber));
            cmd.Parameters.Add(BuildParameter("@Fax", c.General.FaxNumber));
            cmd.Parameters.Add(BuildParameter("@ContactFirstName", c.Contact.FirstName));
            cmd.Parameters.Add(BuildParameter("@ContactLastName", c.Contact.LastName));
            cmd.Parameters.Add(BuildParameter("@ContactPhone", c.Contact.Phone));
            cmd.Parameters.Add(BuildParameter("@ContactExt", c.Contact.PhoneExtension));
            cmd.Parameters.Add(BuildParameter("@Active", c.System.Active));
            cmd.Parameters.Add(BuildParameter("@Need4506", c.ServiceInformation.Requires4506));
            cmd.Parameters.Add(BuildParameter("@NeedSigning", c.ServiceInformation.RequiresSigning));
            cmd.Parameters.Add(BuildParameter("@NeedAppraisal", c.ServiceInformation.RequiresApprasial));
            cmd.Parameters.Add(BuildParameter("@NeedFlood", c.ServiceInformation.RequiresFlood));
            cmd.Parameters.Add(BuildParameter("@NeedTitle", c.ServiceInformation.RequiresTitle));
            cmd.Parameters.Add(BuildParameter("@Price4506", c.ServiceInformation.Price4506.Price));
            cmd.Parameters.Add(BuildParameter("@PriceSigning", c.ServiceInformation.SigningPrices.Price));
            cmd.Parameters.Add(BuildParameter("@SEDoc", c.ServiceInformation.SigningPrices.EDocPrice));
            cmd.Parameters.Add(BuildParameter("@SConcurrent2nd", c.ServiceInformation.SigningPrices.Concurrent2nd));
            cmd.Parameters.Add(BuildParameter("@SAttorneyClosing", c.ServiceInformation.SigningPrices.AttorneyClosing));
            cmd.Parameters.Add(BuildParameter("@SSingleSigning", c.ServiceInformation.SigningPrices.SingleSigning));
            cmd.Parameters.Add(BuildParameter("@SRefusedSigning", c.ServiceInformation.SigningPrices.RefusedSigning));
            cmd.Parameters.Add(BuildParameter("@SNoShow", c.ServiceInformation.SigningPrices.NoShow));
            cmd.Parameters.Add(BuildParameter("@PriceAppraisal", c.ServiceInformation.ApprasialPrices.Price));
            cmd.Parameters.Add(BuildParameter("@PriceFlood", c.ServiceInformation.FloodPrices.Price));
            cmd.Parameters.Add(BuildParameter("@PriceTitle", c.ServiceInformation.TitlePrices.Price));
            cmd.Parameters.Add(BuildParameter("@DepositRequired", c.Deposit.Required));
            cmd.Parameters.Add(BuildParameter("@DepositAmt", c.Deposit.Amount));
            cmd.Parameters.Add(BuildParameter("@CancelOrdersNotifyProcessor", c.Notication.NotifyProcessor));
            cmd.Parameters.Add(BuildParameter("@CancelOrdersNotifySalesRep", c.Notication.NotifyAgentRep));
            cmd.Parameters.Add(BuildParameter("@DocumentEmail", c.System.DocumentEmail));

            cmd.ExecuteNonQuery();

            return new UpsertModelResponse<Company> { Model = c, Success = true, Message = "Company updated" };
        }

        public async Task<UpsertModelResponse<Company>> InsertAsync(Company c)
        {
            using var conn = await GetConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO tblCompany" +
                "(CompanyName,Address,Address2,City,State,Zip,Phone,Fax," +
                "ContactFirstName,ContactLastName,ContactPhone,ContactExt" +
                "Active," +
                "Need4506,NeedSigning,NeedAppraisal,NeedFlood,NeedTitle," +
                "Price4506," +
                "PriceSigning,SEDoc,SConcurrent2nd,SAttorneyClosing,SSingleSigning,SRefusedSigning,SNoShow," +
                "PriceAppraisal,PriceFlood,PriceTitle," +
                "DepositREquired,DepositAmt," +
                "CancelOrdersNotifyProcessor,CancelOrdersNotifySalesRep," +
                "DocumentEmail)" +
                "VALUES" +
                "(@CompanyName,@Address,@Address2,@City,@State,@Zip,@Phone,@Fax," +
                "@ContactFirstName,@ContactLastName,@ContactPhone,@ContactExt" +
                "@Active," +
                "@Need4506,@NeedSigning,@NeedAppraisal,@NeedFlood,@NeedTitle," +
                "@Price4506," +
                "@PriceSigning,@SEDoc,@SConcurrent2nd,@SAttorneyClosing,@SSingleSigning,@SRefusedSigning,@SNoShow," +
                "@PriceAppraisal,@PriceFlood,@PriceTitle," +
                "@DepositREquired,@DepositAmt," +
                "@CancelOrdersNotifyProcessor,@CancelOrdersNotifySalesRep," +
                "@DocumentEmail)";

            cmd.Parameters.Add(BuildParameter("@CompanyName", c.General.Name));
            cmd.Parameters.Add(BuildParameter("@Address", c.General.Address1));
            cmd.Parameters.Add(BuildParameter("@Address2", c.General.Address2));
            cmd.Parameters.Add(BuildParameter("@City", c.General.City));
            cmd.Parameters.Add(BuildParameter("@State", c.General.State));
            cmd.Parameters.Add(BuildParameter("@Zip", c.General.Zip));
            cmd.Parameters.Add(BuildParameter("@Phone", c.General.PhoneNumber));
            cmd.Parameters.Add(BuildParameter("@Fax",c.General.FaxNumber));
            cmd.Parameters.Add(BuildParameter("@ContactFirstName", c.Contact.FirstName));
            cmd.Parameters.Add(BuildParameter("@ContactLastName", c.Contact.LastName));
            cmd.Parameters.Add(BuildParameter("@ContactPhone", c.Contact.Phone));
            cmd.Parameters.Add(BuildParameter("@ContactExt", c.Contact.PhoneExtension));
            cmd.Parameters.Add(BuildParameter("@Active", c.System.Active));
            cmd.Parameters.Add(BuildParameter("@Need4506", c.ServiceInformation.Requires4506));
            cmd.Parameters.Add(BuildParameter("@NeedSigning", c.ServiceInformation.RequiresSigning));
            cmd.Parameters.Add(BuildParameter("@NeedAppraisal", c.ServiceInformation.RequiresApprasial));
            cmd.Parameters.Add(BuildParameter("@NeedFlood", c.ServiceInformation.RequiresFlood));
            cmd.Parameters.Add(BuildParameter("@NeedTitle", c.ServiceInformation.RequiresTitle));
            cmd.Parameters.Add(BuildParameter("@Price4506", c.ServiceInformation.Price4506.Price));
            cmd.Parameters.Add(BuildParameter("@PriceSigning", c.ServiceInformation.SigningPrices.Price));
            cmd.Parameters.Add(BuildParameter("@SEDoc", c.ServiceInformation.SigningPrices.EDocPrice));
            cmd.Parameters.Add(BuildParameter("@SConcurrent2nd", c.ServiceInformation.SigningPrices.Concurrent2nd));
            cmd.Parameters.Add(BuildParameter("@SAttorneyClosing", c.ServiceInformation.SigningPrices.AttorneyClosing));
            cmd.Parameters.Add(BuildParameter("@SSingleSigning", c.ServiceInformation.SigningPrices.SingleSigning));
            cmd.Parameters.Add(BuildParameter("@SRefusedSigning", c.ServiceInformation.SigningPrices.RefusedSigning));
            cmd.Parameters.Add(BuildParameter("@SNoShow", c.ServiceInformation.SigningPrices.NoShow));
            cmd.Parameters.Add(BuildParameter("@PriceAppraisal", c.ServiceInformation.ApprasialPrices.Price));
            cmd.Parameters.Add(BuildParameter("@PriceFlood", c.ServiceInformation.FloodPrices.Price));
            cmd.Parameters.Add(BuildParameter("@PriceTitle", c.ServiceInformation.TitlePrices.Price));
            cmd.Parameters.Add(BuildParameter("@DepositRequired", c.Deposit.Required));
            cmd.Parameters.Add(BuildParameter("@DepositAmt", c.Deposit.Amount));
            cmd.Parameters.Add(BuildParameter("@CancelOrdersNotifyProcessor", c.Notication.NotifyProcessor));
            cmd.Parameters.Add(BuildParameter("@CancelOrdersNotifySalesRep", c.Notication.NotifyAgentRep));
            cmd.Parameters.Add(BuildParameter("@DocumentEmail", c.System.DocumentEmail));

            cmd.ExecuteNonQuery();

            return new UpsertModelResponse<Company> { Model = c, Success = true, Message = "Company added" };
        }
    }    
}
