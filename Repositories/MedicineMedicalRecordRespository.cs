using Dapper;
using DataModels.Config;
using DataModels.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Repositories
{
    public class MedicineMedicalRecordRespository
    {
        private AppDbContext dbContext;
        private DapperContext dapperContext;
        //private ILogger logger;
        public MedicineMedicalRecordRespository(AppDbContext dbContext, DapperContext dapperContext)
        {
            this.dbContext = dbContext;
            this.dapperContext = dapperContext;
            //this.logger = logger;
        }

        public async Task<InformationPrescriptionModel?> GetByKey(int id, int sequence)
        {

            var mr = await dbContext.MedicalRecords.Where(mr => (mr.Id == id && mr.SequenceNumber == sequence))
                .FirstOrDefaultAsync();
      
            var cus = await dbContext.Customers.FindAsync(mr.CustomerId);
            var listMmr = await dbContext.Medicine_MedicalRecords
                .Where(mmr => (mmr.MedicalRecordId == mr.Id && mmr.SequenceNumber == mr.SequenceNumber))
                .Select(mmr => new
                {
                    mmr = mmr,
                    med = dbContext.Medicines.Where(med => med.Id == mmr.MedicineId).SingleOrDefault()
                })
                .ToListAsync();
            if(listMmr == null || listMmr.Count == 0)
            {
                return null;
            }

            var listMyMedicine = new List<MyPrescriptionMedicine>();
            foreach (var item in listMmr)
            {
                listMyMedicine.Add(new MyPrescriptionMedicine()
                {
                    medicine = item.med,
                    medicine_MedicalRecord = item.mmr
                });
            }

            return new InformationPrescriptionModel()
            {
                customer = cus,
                medicalRecord = mr,
                listMedicine = listMyMedicine
            };
        }

        public async Task<(int, List<int>)> Add(FormPrescriptionModel model) 
        {
            //use proc here
            string procedureName = "ADD_MEDICINE_MEDICALRECORD_DEADLOCK3";
			//@MedicineId int, @MedicalRecordId int, @SequenceNumber int, @MedicineQuantityOrigin int
            int check = 2;
            int index = 0;
            List<int> list = new List<int>();
            foreach(var item in model.numberMedicines)
            {
                index++;
                var paramerters = new DynamicParameters();
                paramerters.Add("MedicineId", item.medicineId);
                paramerters.Add("MedicalRecordId", model.id);
                paramerters.Add("SequenceNumber", model.sequence);
                paramerters.Add("MedicineQuantityOrigin", item.quantity);
                using (var connection = dapperContext.CreateConnection())
                {
                    try
                    {
                        await connection.ExecuteAsync(procedureName,
                            paramerters, commandType: CommandType.StoredProcedure);
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteAsync("------------------==================-------------------");
                        await Console.Out.WriteLineAsync(ex.Message);
                        await Console.Out.WriteAsync("------------------==================-------------------");
                        if (ex.Message.Contains("deadlock"))
                        {
                            check = 1;
                        }
                        else
                        {
                            check = 0; //ko tim thay thuoc hoac la thuoc het han
                            list.Add(index);
                        }
                    }
                }
            }
            return (check,list);
		}
    }
}
