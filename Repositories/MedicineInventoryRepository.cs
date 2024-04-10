using Dapper;
using DataModels;
using DataModels.Config;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Repositories
{
    public class MedicineInventoryRepository
	{
		private AppDbContext dbContext;
		private DapperContext dapperContext;
		public MedicineInventoryRepository(AppDbContext dbContext,
			DapperContext dapperContext)
		{
			this.dbContext = dbContext;
			this.dapperContext = dapperContext;
		}

		public async Task<List<MedicineInventoryModel>> GetAllMedicineInventory()
		{
			var data = await dbContext.MedicineInventories.
				Select(mr => new MedicineInventoryModel
				{
					Id = mr.Id,
					MedicineId = mr.MedicineId,
					NameMedicine = dbContext.Medicines.Where(med => med.Id == mr.Medicine.Id).First().Name,
					ExpiryDate = mr.ExpiryDate,
					InventoryQuantity = mr.InventoryQuantity,
					Unit = mr.Unit
				}).ToListAsync();
			return data;
		}

		public async Task InsertMedicineInvetory(MedicineInventory model)
		{
			await dbContext.MedicineInventories.AddAsync(model);
			await dbContext.SaveChangesAsync();
		}

		public async Task<int> Delete(int id)
		{
			var item = await dbContext.MedicineInventories.FindAsync(id);
			if (item != null)
			{
				dbContext.MedicineInventories.Remove(item);
				await dbContext.SaveChangesAsync();
				return 1;
			}
			return 0;
		}

		public async Task<int> Update(MedicineInventory model)
		{
			//var item = await dbContext.MedicineInventories.FindAsync(model.Id);
			//if (item != null)
			//{
			//	dbContext.Entry(item).CurrentValues.SetValues(model);
			//	await dbContext.SaveChangesAsync();
			//}

			int check; //thanh cong
						   //use procedure 
						   //UPDATE_QUANTITY_DEADLOCK2
			var param = new DynamicParameters();
			string procedureName = "UPDATE_QUANTITY_DEADLOCK2";
			//@ID INT, @QUANTITY int
			param.Add("ID", model.Id);
			param.Add("QUANTITY", model.InventoryQuantity);
            param.Add("@check", direction: ParameterDirection.ReturnValue);
            using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection.ExecuteAsync(procedureName, param, commandType: CommandType.StoredProcedure);
					check = param.Get<int>("@check");
				}
				catch (Exception ex)
				{
					check = 1;
				}
			}
			return check;
        }

        public async Task<MedicineInventory?> GetByKey(int id)
		{
			return await dbContext.MedicineInventories.FindAsync(id);
		}

		public async Task<int> DeleteAllExpirededicine()
		{
			string procName = "DELETE_EXPIRED_MEDICINE_DEADLOCK2";
			var param = new DynamicParameters();
			int check;
			param.Add("@check", direction: ParameterDirection.ReturnValue);
			using(var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection.ExecuteAsync(procName, param, 
						commandType : CommandType.StoredProcedure);
					check = param.Get<int>("@check");
				}
				catch(Exception ex)
				{
                    await Console.Out.WriteLineAsync("---------------------================-----------------");
                    await Console.Out.WriteLineAsync(ex.Message);
                    await Console.Out.WriteLineAsync("---------------------================-----------------");
					check = 1;
				}
			}
			return check;
		}

		public async Task<int> IncreaseMedicine(int id)
		{
			string procName = "Them1Thuoc";
			var param = new DynamicParameters();
			param.Add("Id", id, DbType.Int32);
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection.ExecuteAsync(procName, param,
						commandType: CommandType.StoredProcedure);
				}
				catch (Exception ex)
				{
					await Console.Out.WriteLineAsync("---------------------================-----------------");
					await Console.Out.WriteLineAsync(ex.Message);
					await Console.Out.WriteLineAsync("---------------------================-----------------");
					
				}
			}
			var res = await dbContext.MedicineInventories.
				Where(x => x.Id == id).SingleOrDefaultAsync();
			return res.InventoryQuantity;
		}

		public async Task<int> DecreaseMedicine(int id)
		{
			string procName = "Giam1Thuoc";
			var param = new DynamicParameters();
			param.Add("Id", id, DbType.Int32);
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection.ExecuteAsync(procName, param,
						commandType: CommandType.StoredProcedure);
				}
				catch (Exception ex)
				{
					await Console.Out.WriteLineAsync("---------------------================-----------------");
					await Console.Out.WriteLineAsync(ex.Message);
					await Console.Out.WriteLineAsync("---------------------================-----------------");

				}
			}
			var res = await dbContext.MedicineInventories.
				Where(x => x.Id == id).SingleOrDefaultAsync();
			return res.InventoryQuantity;
		}
	}
}