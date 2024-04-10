using AutoMapper;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
	[Authorize(Roles = "Admin")]
	public class MedicineInventoryController : Controller
	{
		private IMapper mapper;
		private MedicineInventoryRepository medicineInventoryRepository;
		private MedicineRepository medicineRepository;
		public MedicineInventoryController(IMapper mapper,
			MedicineInventoryRepository medicineInventoryRepository,
			MedicineRepository medicineRepository)
		{

			this.mapper = mapper;
			this.medicineInventoryRepository = medicineInventoryRepository;
			this.medicineRepository = medicineRepository;
		}
		
		public async Task<IActionResult> Index()
		{
			var dataRaw = await medicineInventoryRepository.GetAllMedicineInventory();
			return View(dataRaw);
		}

		public async Task<IActionResult> Create()
		{
			var rawData = (await medicineRepository.GetAllMedicine("")).Item1;
			var listItem = new List<SelectListItem>();
			foreach (var item in rawData)
			{
				listItem.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString()});
			}
			ViewBag.ListMidicine = listItem;
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(WebApplication.Models.MedicineInventoryModel model)
		{
			if (ModelState.IsValid)
			{
				MedicineInventory item = mapper.Map<MedicineInventory>(model);
				await medicineInventoryRepository.InsertMedicineInvetory(item);
				return RedirectToAction("Index");
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await medicineInventoryRepository.Delete(id);
			if(result == 1)
			{
				return Ok();
			}
			return BadRequest();
		}

		public async Task<IActionResult> Edit(int id)
		{
			var targerMedicineInventory = await medicineInventoryRepository.GetByKey(id);
			
			return View(targerMedicineInventory);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(WebApplication.Models.MedicineInventoryModel model)
		{
			if(ModelState.IsValid)
			{
				var result = await medicineInventoryRepository.Update(mapper.Map<MedicineInventory>(model));
                if (result == 1)
                {
                    TempData["Err"] = "Cập nhật số lượng thuốc thất bại vì xảy ra deadlock";
                }
                else
                {
                    TempData["Success"] = "Cập nhật số lượng thuốc hết hạn thành công";
                }
                return Redirect("/MedicineInventory/Index");
			}
			return View(model);
		}

		public async Task<IActionResult> DeleteAllExpirededicine()
		{
			var result = await medicineInventoryRepository.DeleteAllExpirededicine();
			if (result == 1)
			{
				TempData["Err"] = "Xóa thuốc hết hạn thất bại vì xảy ra deadlock";
			}
			else
			{
				TempData["Success"] = "Xóa thuốc hết hạn thành công";
			}
			return Redirect("/MedicineInventory/Index");
		}
		[HttpPost]
		public async Task<IActionResult> IncreaseMedicine(int id)
		{
			var quantity = await medicineInventoryRepository.IncreaseMedicine(id);
			return Ok(quantity);
		}

		[HttpPost]
		public async Task<IActionResult> DecreaseMedicine(int id)
		{
			var quantity = await medicineInventoryRepository.DecreaseMedicine(id);
			return Ok(quantity);
		}
	}
}