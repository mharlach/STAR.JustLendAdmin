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
    public class Vendor4506Service : DbContextBase, IDetailedVendorService
    {
        private ILogger<Vendor4506Service> log;

        public Vendor4506Service(ILogger<Vendor4506Service> log, IConfiguration configuration) : base(configuration)
        {
            this.log = log;
        }

        public async Task<IEnumerable<Vendor>> GetAsync()
        {
            var conn = await GetConnectionAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {GetTableName()}";
            var rdr = cmd.ExecuteReader();
            var vendors = new List<Vendor>();
            while (rdr.Read())
            {
                vendors.Add(Read(rdr));
            }

            return vendors;
        }

        private Vendor Read(System.Data.IDataReader rdr)
        {
            var vendor = new Vendor
            {
                Id = (int)rdr["ID"],
                VendorType = GetVendorType(),
                General = new GeneralData
                {
                    Name = rdr["CompanyName"] as string ?? string.Empty,
                    Address1 = rdr["Address"] as string ?? string.Empty,
                    Address2 = rdr["Address2"] as string ?? string.Empty,
                    City = rdr["City"] as string ?? string.Empty,
                    State = rdr["State"] as string ?? string.Empty,
                    PhoneNumber = rdr["Phone"] as string ?? string.Empty,
                    FaxNumber = rdr["Fax"] as string ?? string.Empty,
                    Zip = rdr["Zip"] as string ?? string.Empty,
                },
                Contact = new VendorContact
                {
                    Name = rdr["ContactName"] as string ?? string.Empty,
                    Phone = rdr["ContactPhone"] as string ?? string.Empty,
                    PhoneExtention = rdr["ContactExt"] as string ?? string.Empty,
                    Email = rdr["ContactEmail"] as string ?? string.Empty,
                },
                SystemData = new VendorSystemData
                {
                    AdminId = rdr["AdminID"] as string ?? string.Empty,
                    AdminPassword = rdr["AdminPassword"] as string ?? string.Empty,
                    Active = (bool)rdr["Active"],
                    MolCompanyCode = rdr["MOLCompCode"] as string ?? string.Empty,
                },
                ServiceInformation = new VendorServiceInformation
                {
                    Pricing = new Pricing
                    {
                        Price = rdr["Price"] as decimal? ?? 0,
                    },
                },
            }
        }

        public async Task<Vendor?> GetAsync(int id)
        {
            var conn = await GetConnectionAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {GetTableName()} WHERE ID=@ID";
            cmd.Parameters.Add(BuildParameter("@ID", id));
            var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return Read(rdr);
            }
            else
            {
                return null;
            }
        }

        private string GetTableName() => "tblVendor4506";

        public VendorType GetVendorType() => VendorType.FourFiveZeroSix;

        public Task<UpsertModelResponse<Vendor>> UpsertAsync(Vendor vendor)
        {
            throw new NotImplementedException();
        }
    }
}
