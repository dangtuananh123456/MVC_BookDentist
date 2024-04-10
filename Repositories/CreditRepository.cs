using Dapper;
using DataModels.Config;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CreditRepository
	{
		private AppDbContext dbContext;
		private DapperContext dapper;
		public CreditRepository(AppDbContext dbContext, DapperContext dapper)
		{
			this.dbContext = dbContext;
			this.dapper = dapper;
		}

		public async Task<int> Transfer(string customerid, decimal total)
		{
			var param = new DynamicParameters();
			param.Add("customerId", customerid);
			param.Add("number", total);
			string nameProc = "lostupdate_credit_minus";
			using(var connection = dapper.CreateConnection())
			{
				try
				{
					await connection.ExecuteAsync(nameProc, param, commandType : CommandType.StoredProcedure);
					return 1;
				}
				catch (Exception ex)
				{
                    await Console.Out.WriteLineAsync(ex.Message);
                    return 0;
				}
			}
		}
		
		public async Task AddMoney(string customerid, decimal number)
		{
			var param = new DynamicParameters();
			string nameProc = "lostupdate_credit_add";
			param.Add("customerId", customerid);
			param.Add("number", number);
			using(var connection = dapper.CreateConnection())
			{
				try
				{
					await connection.ExecuteAsync(nameProc, param, commandType: CommandType.StoredProcedure);
				}
				catch(Exception ex)
				{
                    await Console.Out.WriteLineAsync(ex.Message);
                }
			}
		}

		public async Task<decimal> GetBalance(string customerId)
		{
			var wallet = await dbContext.Credits
				.Where(x => x.CustomerId == customerId)
				.SingleOrDefaultAsync();
			return wallet.Balance;
		}
	}
}
