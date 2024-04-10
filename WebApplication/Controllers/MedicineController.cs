using AutoMapper;
using DataModels;
using DataModels.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "Admin")]
	public class MedicineController : Controller
    {
        private AppDbContext dbContext;
        private MedicineRepository medicineRepository;
        private IMapper mapper;
        public MedicineController(AppDbContext dbContext, MedicineRepository medicine, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.medicineRepository = medicine;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index(string text = "")
        {
            var result = await medicineRepository.GetAllMedicine(text);
            var item = result.Item1;
            ViewData["Count"] = result.Item2;
            ViewData["Text"] = text;
            List<CreateMedicineModel> model = new List<CreateMedicineModel>();
            for (int i = 0; i < item.Count(); i++)
            {
                model.Add(mapper.Map<CreateMedicineModel>(item[i]));
            }
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMedicineModel model)
        {
            if (ModelState.IsValid)
            {
                Medicine item = mapper.Map<Medicine>(model);
                await medicineRepository.AddMedicine(item);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await medicineRepository.GetMedicineById(id);
            ViewData["id"] = id;

			return View(mapper.Map<CreateMedicineModel>(medicine));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateMedicineModel model)
        {
            if (ModelState.IsValid)
            {
                await medicineRepository.UpdateMedicine(mapper.Map<Medicine>(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await medicineRepository.RemoveMedicine(id);
            if(result == 1)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}