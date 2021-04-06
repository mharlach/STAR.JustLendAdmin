#nullable enable

using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace STAR.JustLendAdmin.Web.Services
{
    public abstract class DbContextBase
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public DbContextBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration["SqlConnectionString"];
        }

        protected async Task<IDbConnection> GetConnectionAsync()
        {
            var conn = new SqlConnection(this.connectionString);
            await conn.OpenAsync();
            return conn;
        }

        protected IDbDataParameter BuildParameter(string paramName, object? paramValue) => new SqlParameter(paramName, paramValue);
    }
}