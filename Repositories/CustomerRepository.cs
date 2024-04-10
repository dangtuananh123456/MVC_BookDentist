using Dapper;
using DataModels;
using DataModels.Config;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Data;
using Microsoft.Identity.Client;
using DataModels.DbContexts;

namespace Repositories
{
    public class CustomerRepository
    {
        private readonly AppDbContext dbContext;
        private readonly DapperContext dapperContext;

        public CustomerRepository(AppDbContext dbContext, DapperContext dapperContext)
        {
            this.dbContext = dbContext;
            this.dapperContext = dapperContext;
        }

        public async Task<List<Customer>> GetAll ()
        {
            return await dbContext.Customers.ToListAsync();
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await dbContext.Customers.AddAsync(customer);
            var credit = new Credit()
            {
                Balance = 0,
                CustomerId = customer.Id,
            };
            await dbContext.Credits.AddAsync(credit);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var param = new DynamicParameters();
            string procedureName = "ChinhSuaThongTinKH";
            param.Add("Makh", customer.Id);
            param.Add("Tenkh", customer.FullName);
            param.Add("Diachi", customer.Address);
            param.Add("NgaySinh", customer.DayOfBirth);
            using (var connection = dapperContext.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(procedureName, param, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    await Console.Out.WriteLineAsync(ex.Message);
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    
                }
            }
        }

        public async Task<Customer?> GetCustomerByAccountAsync(AppUser user)
		{

            return await dbContext.Customers.Where(x => x.AccountId == user.Id).SingleOrDefaultAsync();
		}

		public async Task<(Customer, int)> GetCustomerByNameAsync(string customerName)
		{
            int timthay = 0;
            var param = new DynamicParameters();
            string procedureName = "XemThongTinKhachHang";
            param.Add("tenKH", customerName, DbType.String);
            param.Add("timthay", customerName, DbType.Int32, direction: ParameterDirection.Output);
            using (var connection = dapperContext.CreateConnection())
            {
                try
                {
                    var result = await connection
                        .QueryAsync<Customer>(procedureName, param, commandType: CommandType.StoredProcedure);
					timthay = param.Get<int>("timthay");
					return (result.SingleOrDefault(), timthay);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    await Console.Out.WriteLineAsync(ex.Message);
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    return (null, timthay);
                }
            }
            

        }

		public async Task<Customer?> GetCustomerByIdAsync(string id)
		{

			return await dbContext.Customers.FindAsync(id);
		}
        public async Task<Customer?> GetCustomerByPhoneNumber(string phoneNumber )
        {
            var target = await dbContext.Customers.Where(c => c.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();
            return target;
        }
	}
}
