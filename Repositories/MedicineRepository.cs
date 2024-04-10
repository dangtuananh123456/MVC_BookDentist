using AutoMapper;
using Dapper;
using DataModels;
using DataModels.Config;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Repositories
{
    public class MedicineRepository
    {
        private AppDbContext dbContext;
        private IMapper mapper;
        private readonly DapperContext dapperContext;

        public MedicineRepository(AppDbContext dbContext, IMapper mapper, DapperContext dapperContext)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.dapperContext = dapperContext;
        }
        public async Task<(List<Medicine>, int)> GetAllMedicine(string text)
        {
            List<Medicine> list = new List<Medicine>();
            var param = new DynamicParameters();
            int count = 0;
            string procedureName = "TimKiemDanhMucThuoc";
            param.Add("TenThuoc", text);
			param.Add("count", dbType: DbType.Int32, direction: ParameterDirection.Output);
			using (var connection = dapperContext.CreateConnection())
            {
                try
                {
                    var records = await connection
                        .QueryAsync<Medicine>(procedureName, param, commandType: CommandType.StoredProcedure);
                    list = records.ToList();
					count = param.Get<int>("count");
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

        public async Task<Medicine?> GetMedicineById(int id)
        {
            return await dbContext.Medicines.FindAsync(id);
        }

        public async Task UpdateMedicine(Medicine medicine)
        {
            var check = await dbContext.Medicines.FindAsync(medicine.Id);
            if (check != null)
            {
                dbContext.Medicines.Entry(check).CurrentValues.SetValues(medicine);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> RemoveMedicine(int id)
        {
            var item = await dbContext.Medicines.FindAsync(id);
            if (item != null)
            {
                dbContext.Medicines.Remove(item);
                await dbContext.SaveChangesAsync();
                return 1;
            }
            return 0;
        }

        public async Task<int> AddMedicine(Medicine model)
        {
			var param = new DynamicParameters();
			string procedureName = "ThemDanhMucThuoc";
			param.Add("TenThuoc", model.Name, DbType.String);
			param.Add("ChiTietThuoc", model.Prescription, DbType.String);
			param.Add("GiaTien", model.Price, DbType.Decimal);
			using (var connection = dapperContext.CreateConnection())
			{
				try
				{
					await connection
						.ExecuteAsync(procedureName, param, commandType: CommandType.StoredProcedure);
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

        public async Task<List<(Medicine, int)>> GetAllQuantityInventory()
        {
            var resultQuery = await dbContext.MedicineInventories
                .Where(mi => mi.ExpiryDate > DateOnly.FromDateTime(DateTime.Now))
                .GroupBy(mi => mi.MedicineId)
                .Select(group => new
                {
                    sum = group.Sum(x => x.InventoryQuantity),
                    med = dbContext.Medicines.Where(med => med.Id == group.Key).FirstOrDefault()
                })
                .ToListAsync();
            var final = new List<(Medicine, int)>();
            foreach (var item in resultQuery)
            {
                final.Add((item.med, item.sum));
            }
            return final;
        }
    }
}