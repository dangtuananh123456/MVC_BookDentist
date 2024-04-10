using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataModels.Config
{
    public class DapperContext
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlServer");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

    }
}
