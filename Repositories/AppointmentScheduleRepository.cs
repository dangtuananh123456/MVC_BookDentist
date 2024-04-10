using Dapper;
using DataModels;
using DataModels.Config;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Repositories
{
    public class AppointmentScheduleRepository
	{
		private readonly AppDbContext dbContext;
		private readonly DapperContext dapperContext;

		public AppointmentScheduleRepository(AppDbContext dbContext, DapperContext dapperContext)
		{
			this.dbContext = dbContext;
			this.dapperContext = dapperContext;
		}

        public async Task<List<AppointmentSchedule>> GetAppointmentsOfDentist(string dentistId)
		{
			List<AppointmentSchedule> list = new List<AppointmentSchedule>();
			var param = new DynamicParameters();
			string procedureName = "Xem_Lich_Hen";
			param.Add("IdBS", dentistId);
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					list = (await connection
						.QueryAsync<AppointmentSchedule>(procedureName, param, commandType: CommandType.StoredProcedure)).ToList();
				}
				catch (Exception ex)
				{
					await Console.Out.WriteLineAsync("---------=====================----------------");
					await Console.Out.WriteLineAsync(ex.Message);
					await Console.Out.WriteLineAsync("---------=====================----------------");
					return null;
				}
			}
			return list;
		}

        public async Task<List<AppointmentSchedule>> GetAppointmentsOfCustomer(string customerId)
        {
            return await dbContext.AppointmentSchedules
                .Where(x => !string.IsNullOrEmpty(x.CustomerId) 
				&& x.CustomerId == customerId
				&& x.StartTime >= DateTime.Now)
				.ToListAsync();
        }

        public async Task<int> AddAppoinment(AppointmentSchedule appointment)
        {
			int res = 0;
			var param = new DynamicParameters();
			string procedureName = "Them_Lich_Hen";
			param.Add("IdBS", appointment.DentistId, DbType.String);
			param.Add("IdBN", appointment.CustomerId, DbType.String);
			param.Add("batdau", appointment.StartTime, DbType.DateTime);
			param.Add("ketthuc", appointment.EndTime, DbType.DateTime);
			param.Add("ketqua", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
			SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection
						.ExecuteAsync(procedureName, param, commandType: CommandType.StoredProcedure);
					res = param.Get<int>("ketqua");
				}
				catch (Exception ex)
				{
					await Console.Out.WriteLineAsync("---------=====================----------------");
					await Console.Out.WriteLineAsync(ex.Message);
					await Console.Out.WriteLineAsync("---------=====================----------------");
					return 0;
				}
			}
			return res;

		}

		public async Task<AppointmentSchedule> FindAsync(string dentistId, DateTime startTime)
		{
			return await dbContext.AppointmentSchedules
				.Where(x => x.DentistId == dentistId && x.StartTime == startTime).SingleOrDefaultAsync();
		}

		public async Task UpdateAsync(AppointmentSchedule schedule, DateTime sTime, DateTime eTime, string dentistId = null)
		{
			var newSchedule = new AppointmentSchedule()
			{
				StartTime = sTime,
				EndTime = eTime,
				CustomerId = schedule.CustomerId
			};
			if (dentistId == null)
			{
				newSchedule.DentistId = schedule.DentistId;
			}
			else
			{
				newSchedule.DentistId = dentistId;
			}
            dbContext.AppointmentSchedules.Remove(schedule);
            await dbContext.SaveChangesAsync();
            dbContext.AppointmentSchedules.Add(newSchedule);
			await dbContext.SaveChangesAsync();
		}
	}
}
