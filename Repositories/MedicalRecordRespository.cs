using Dapper;
using DataModels;
using DataModels.Config;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Repositories
{
    public class MedicalRecordRespository
    {
        private AppDbContext dbContext;
        private readonly DapperContext dapperContext;

        public MedicalRecordRespository(AppDbContext dbContext, DapperContext dapperContext)
        {
            this.dbContext = dbContext;
            this.dapperContext = dapperContext;
        }

        public async Task<MedicalRecord?> GetMedicalRecordByPhoneCustomer(string PhoneNumber)
        {
            var target = await dbContext.Customers.Where(c => c.PhoneNumber == PhoneNumber)
            .FirstOrDefaultAsync();
            var final = await dbContext.MedicalRecords.Where(mr => mr.CustomerId == target.Id)
                .FirstOrDefaultAsync();
            return final;
        }
        
        public async Task<(List<MedicalRecord>, int)> GetByIdDentist(string IdDentist, string text)
        {
			List<MedicalRecord> list = new List<MedicalRecord>();
			var param = new DynamicParameters();
			int count = 0;
			string procedureName = "TimKiemHSBA";
			param.Add("dentistId", IdDentist);
			param.Add("phoneNumber", text);
			param.Add("soLuongTimThay", dbType: DbType.Int32, direction: ParameterDirection.Output);
			SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					var records = await connection
						.QueryAsync<MedicalRecord>(procedureName, param, commandType: CommandType.StoredProcedure);
					list = records.ToList();
					count = param.Get<int>("soLuongTimThay");
				}
				catch (Exception ex)
				{
					await Console.Out.WriteLineAsync("---------=====================----------------");
					await Console.Out.WriteLineAsync(ex.Message);
					await Console.Out.WriteLineAsync("---------=====================----------------");
					return (list, -1);
				}
			}
			return (list, count);
		}

        public async Task<MedicalRecord?> GetById(int id, int sn)
        {
            MedicalRecord result = null;
            var param = new DynamicParameters();
            string procedureName = "EMPLOYEE_SEE_RECORD";
            param.Add("id", id);
            param.Add("lankham", sn);
            SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
            using (var connection = dapperContext.CreateConnection())
            {
                try
                {
                    var records = await connection
                        .QueryAsync<MedicalRecord>(procedureName, param, commandType: CommandType.StoredProcedure);
                    result = records.SingleOrDefault();
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    await Console.Out.WriteLineAsync(ex.Message);
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    return null;
                }
            }
            return result;
        }

        public async Task<(List<MedicalRecord>, int)> GetByIdCustomer(string customerId)
        {
            int count = 0;
            List<MedicalRecord> result = new List<MedicalRecord>();
            var param = new DynamicParameters();
            string procedureName = "CUSTOMER_SEE_RECORD";
            param.Add("idCustomer", customerId);
            param.Add("soHS", DbType.Int32, direction: ParameterDirection.Output);
            SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
            using (var connection = dapperContext.CreateConnection())
            {
                try
                {
                    var records = await connection
                        .QueryAsync<MedicalRecord>(procedureName, param, commandType: CommandType.StoredProcedure);
                    result = records.ToList();
                    count = param.Get<int>("soHS");
				}
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    await Console.Out.WriteLineAsync(ex.Message);
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    return (result, count);
                }
            }
            return (result, count);
		}
        public async Task<int> Add(MedicalRecord model)
        {
			var param = new DynamicParameters();
			string procedureName = "ThemHSBA";
			param.Add("id", model.Id, DbType.Int32);
			param.Add("lanKham", model.SequenceNumber, DbType.Int32);
			param.Add("ngayKham", model.ExaminationDate, DbType.Date);
			param.Add("dichVu", model.Service, DbType.String);
			param.Add("gia", model.ServicePrice, DbType.Decimal);
			param.Add("status", model.Status, DbType.String);
			param.Add("customerId", model.CustomerId, DbType.String);
			param.Add("idNhaSiTao", model.CreatedByDentistId, DbType.String);
			param.Add("idNhaSiKham", model.ExamDentistId, DbType.String);
			SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
            int kq;
			param.Add("@kq", direction: ParameterDirection.ReturnValue);
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection
						.ExecuteAsync(procedureName, param, commandType: CommandType.StoredProcedure);
                    kq = param.Get<int>("@kq");
				}
				catch (Exception ex)
				{
					await Console.Out.WriteLineAsync("---------=====================----------------");
					await Console.Out.WriteLineAsync(ex.Message);
					await Console.Out.WriteLineAsync("---------=====================----------------");
					return 1;
				}
			}
			return 0;
		}

        public async Task<int> Delete(int id, int sequence)
        {
			List<MedicalRecord> result = new List<MedicalRecord>();
			var param = new DynamicParameters();
			string procedureName = "DELETE_RECORD";
			param.Add("idHS", id);
			param.Add("lankham", sequence);
			SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection
						.QueryAsync<MedicalRecord>(procedureName, param, commandType: CommandType.StoredProcedure);
                    
				}
				catch (Exception ex)
				{
					await Console.Out.WriteLineAsync("---------=====================----------------");
					await Console.Out.WriteLineAsync(ex.Message);
					await Console.Out.WriteLineAsync("---------=====================----------------");
                    return 0;
				}
			}
			return 1;
		}

        public async Task<int> Edit(MedicalRecord model)
        {
            int kq = 0;
            var param = new DynamicParameters();
            string procedureName = "UPDATE_PATIENT_RECORD";
            param.Add("idHS", model.Id, DbType.Int32);
            param.Add("lankham", model.SequenceNumber, DbType.Int32);
            param.Add("date_time", model.ExaminationDate, DbType.Date);
            param.Add("dichvu", model.Service, DbType.String);
            param.Add("giadichvu", model.ServicePrice, DbType.Decimal);
            param.Add("idCustomer", model.CustomerId, DbType.String);
            param.Add("idBsTao", model.CreatedByDentistId, DbType.String);
            param.Add("idBsKT", model.ExamDentistId, DbType.String);
            param.Add("kq", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
            using (var connection = dapperContext.CreateConnection())
            {
                try
                {
                    await connection
                        .ExecuteAsync(procedureName, param, commandType: CommandType.StoredProcedure);
                    kq = param.Get<int>("kq");
                }

                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    await Console.Out.WriteLineAsync(ex.Message);
                    await Console.Out.WriteLineAsync("---------=====================----------------");
                    return kq;
                }
            }
            return kq;
            
        }

        public async Task<MedicalRecord?> GetLatestMedicalRecordByCustomerId(string customerId)
        {
            if(await dbContext.MedicalRecords.CountAsync() == 0)
            {
                return null;
            }
            var target = await dbContext.MedicalRecords.Where(mr => mr.CustomerId == customerId)
                .OrderByDescending(mr => mr.SequenceNumber).FirstOrDefaultAsync();
            return target;
        }

        public async Task<MedicalRecord?> GetMaxId()
        {
            var target = await dbContext.MedicalRecords.OrderByDescending(m => m.Id).FirstOrDefaultAsync();
            return target;
        }

       
    }
}
