using DataModels.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repositories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class PaymentController : Controller
	{
		private MedicalRecordRespository medicalRecordRespository;
		private AppDbContext appDbContext;
		private CustomerRepository customerRepository;
		private CreditRepository creditRepository;
		public PaymentController(MedicalRecordRespository medicalRecordRespository
			, AppDbContext appDbContext, CustomerRepository customerRepository
			,CreditRepository creditRepository)
		{	
			this.medicalRecordRespository = medicalRecordRespository;
			this.appDbContext = appDbContext;
			this.customerRepository = customerRepository;
			this.creditRepository = creditRepository;
		}
		public async Task<IActionResult> Index()
		{
			var result = await customerRepository.GetAll();
			var result1 = from c in result.ToList()
						  join m in appDbContext.MedicalRecords
						  on c.Id equals m.CustomerId into mytable
						  select c;
			result1 = result1.Distinct().ToList();
			var list = new List<SelectListItem>();
			foreach (var item in result1)
			{
				list.Add(new SelectListItem()
				{
					Value = item.Id,
					Text = item.PhoneNumber
				});
			}
			ViewBag.list = list;
			return View();
		}

		public async Task<IActionResult> Bill(string customerId, string statuspayment)
		{
			BillModel model = new BillModel();
			model.statuspayment = statuspayment;
			model.CustomerId = customerId;
			var medicalRecord = await appDbContext.MedicalRecords.
				Where(mr => mr.CustomerId == customerId).OrderByDescending(mr => mr.SequenceNumber).FirstOrDefaultAsync();
			if(medicalRecord != null)
			{
				var getListMedicine = await appDbContext.Medicine_MedicalRecords
					.Where(item => (item.MedicalRecordId == medicalRecord.Id && item.SequenceNumber == medicalRecord.SequenceNumber))
					.Select(item => new
					{
						quantity = item.MedicineQuantity,
						medicine = appDbContext.Medicines.Where(m => m.Id == item.MedicineId).FirstOrDefault()
					}).ToListAsync();
				model.NameService = medicalRecord.Service;
				model.SerVicePrice = medicalRecord.ServicePrice;
				
				//create bill model
				if(getListMedicine == null || getListMedicine.Count == 0) {
					return View(model);
				}
				model.Status = medicalRecord.Status;
                decimal Total = medicalRecord.ServicePrice;
				model.IdMedicalRecord = medicalRecord.Id;
				model.Sequence = medicalRecord.SequenceNumber;
				model.medicines = new List<MyMedicine>();
				foreach (var item in getListMedicine)
				{
					model.medicines.Add(new MyMedicine()
					{
						Name = item.medicine.Name,
						Quantity = item.quantity,
						Price = item.medicine.Price
					});
					Total += item.quantity * item.medicine.Price;
				}
				model.Total = Total;
			}
			return View(model);
		}

		public async Task<IActionResult> Transfer(string customerid, decimal total, int id, int sequence)
		{
			var resultTransfer = await creditRepository.Transfer(customerid, total);
			if(resultTransfer == 1)
			{
				//update trang thai da thanh toan chi phi
				var target = await appDbContext.MedicalRecords
					.Where(mr => (mr.Id == id && mr.SequenceNumber == sequence))
					.FirstOrDefaultAsync();
				var temp = target;
				temp.Status = "yes";
				appDbContext.MedicalRecords.Entry(target).CurrentValues.SetValues(temp);
				await appDbContext.SaveChangesAsync();
				return Redirect($"/Payment/Bill?customerId={customerid}&statuspayment=no");
			}
			else
			{
				return Redirect($"/Payment/Bill?customerId={customerid}&statuspayment=yes");
			}
		}

	}
}
