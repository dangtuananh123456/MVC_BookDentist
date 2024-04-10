using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace WebApplication.Controllers
{
	public class PrescriptionController : Controller
	{
		private MedicineMedicalRecordRespository medicineMedicalRecordRespository;
		private MedicineRepository medicineRepository;
		public PrescriptionController(MedicineMedicalRecordRespository medicineMedicalRecordRespository
			, MedicineRepository medicineRepository)
		{
			this.medicineMedicalRecordRespository = medicineMedicalRecordRespository;
			this.medicineRepository = medicineRepository;
		}

		public async Task<ActionResult> Index(int id, int sequence)
		{
			ViewBag.id = id;
			ViewBag.sequence = sequence;
			var result = await medicineMedicalRecordRespository.GetByKey(id, sequence);
			if (result == null)
			{
				ViewBag.check = false;
			}
			else
			{
				ViewBag.check = true;
			}
			return View(result);
		}

		public async Task<IActionResult> Create(int id, int sequence)
		{
			ViewBag.id = id;
			ViewBag.sequence = sequence;
			var listMed = await medicineRepository.GetAllQuantityInventory();
			return View(listMed);
		}

		[HttpPost]
		public async Task<IActionResult> Create(int id, int sequence, List<NumberMedicine> numberMedicines)
		{
            if(ModelState.IsValid)
            {
			    (int check,List<int> list) = await medicineMedicalRecordRespository.Add(new FormPrescriptionModel()
				{
					id = id,
					sequence = sequence,
					numberMedicines = numberMedicines
				} );
				if(check == 2)
				{
					return Ok(new {status = 200});
				}
				else 
				{
					if(check == 1)
					{
                        return Ok(new { status = 400}); //loi deadlock
                    }
					else
					{
						return Ok(new {status = 401, list = list}); //loi het han
					}
				}
			}
            ViewBag.id = id;
            ViewBag.sequence = sequence;
            var listMed = await medicineRepository.GetAllQuantityInventory();
            return View(listMed);
		}
	}
}
