using AutoMapper;
using DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MedicalRecordController : Controller
    {
        private IMapper mapper;
        private MedicalRecordRespository medicalRecordRespository;
        private UserManager<AppUser> userManager;
        private CustomerRepository customerRepository;
        private ILogger<MedicalRecordController> logger;
        private DentistRepository dentistRepository;

        public MedicalRecordController(IMapper mapper,
			MedicalRecordRespository medicalRecordRespository,
			CustomerRepository customerRepository, UserManager<AppUser> userManager,
			ILogger<MedicalRecordController> logger, DentistRepository dentistRepository)
		{
			this.mapper = mapper;
			this.medicalRecordRespository = medicalRecordRespository;
			this.customerRepository = customerRepository;
			this.userManager = userManager;
			this.logger = logger;
			this.dentistRepository = dentistRepository;
		}

		public async Task<IActionResult> Index(string dentistId = "", string text = "")
        {
            ViewBag.id = dentistId;
            await Console.Out.WriteLineAsync("======================================");
            await Console.Out.WriteLineAsync($"dentist id => {dentistId}");
            await Console.Out.WriteLineAsync("======================================");
			var result = await medicalRecordRespository.GetByIdDentist(dentistId, text);
			var items = result.Item1;
			ViewData["Count"] = result.Item2;
			ViewData["Text"] = text;
			return View(items);
        }

		public async Task<IActionResult> MedicalRecordOfCustomer(string customerId)
        {
            
            var result = await medicalRecordRespository.GetByIdCustomer(customerId);
            var model = result.Item1;
			@ViewData["Count"] = result.Item2;

			return View("Index", model);
        }

        public async Task<IActionResult> AddMedicalRecord(string id)
        {
            ViewBag.id = id;
            ViewBag.SelectService = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Khám tổng quát", Value = "Khám tổng quát"},
                new SelectListItem() {Text = "Nội soi", Value = "Nội soi"},
                new SelectListItem() {Text = "Siêu âm", Value = "Siêu âm"}
            };

            var SelectPhoneNumber = new List<SelectListItem>();
            var listPhone = await customerRepository.GetAll();
            foreach (var item in listPhone)
            {
                SelectPhoneNumber.Add(new SelectListItem() { Text = item.PhoneNumber, Value = item.PhoneNumber });
            }
            ViewBag.SelectPhoneNumber = SelectPhoneNumber;
            var model = new MedicalRecordModel()
            {
                ExaminationDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMedicalRecord(MedicalRecordModel model)
        {
            if (ModelState.IsValid)
            {
                
                var item = new MedicalRecord();
                item.CreatedByDentistId = model.CreatedByDentistId;
                item.ExamDentistId = model.CreatedByDentistId;
                item.ExaminationDate = new DateOnly(model.ExaminationDate.Year, model.ExaminationDate.Month, model.ExaminationDate.Day);
                item.Service = model.Service;
                var targetCus = await customerRepository.GetCustomerByPhoneNumber(model.PhoneNumber.Trim());
                if(targetCus == null)
                {
                    ModelState.AddModelError(string.Empty, $"{model.PhoneNumber} không tồn tại");
                    return View("error");
                }
                item.CustomerId = targetCus.Id;
                item.ServicePrice = 100000;
                var recordMedical = await medicalRecordRespository.GetLatestMedicalRecordByCustomerId(item.CustomerId);
                if(recordMedical == null)
                {
                    var medicalRecord = await medicalRecordRespository.GetMaxId();
                    if(medicalRecord != null)
                    {
                        item.Id = medicalRecord.Id + 1;
                        item.SequenceNumber = 1;
                    }
                    else
                    {
                        item.Id = 1;
                        item.SequenceNumber = 1;
                    }
                }
                else
                {
                    item.Id = recordMedical.Id;
                    item.SequenceNumber = recordMedical.SequenceNumber + 1;
                }
                await medicalRecordRespository.Add(item);
                return Redirect($"/MedicalRecord/Index?dentistId={model.CreatedByDentistId}");
            }
            ViewBag.SelectService = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Khám tổng quát", Value = "Khám tổng quát"},
                new SelectListItem() {Text = "Nội soi", Value = "Nội soi"},
                new SelectListItem() {Text = "Siêu âm", Value = "Siêu âm"}
            };

            var SelectPhoneNumber = new List<SelectListItem>();
            var listPhone = await customerRepository.GetAll();
            foreach (var item in listPhone)
            {
                SelectPhoneNumber.Add(new SelectListItem() { Text = item.PhoneNumber, Value = item.PhoneNumber });
            }
            ViewBag.SelectPhoneNumber = SelectPhoneNumber;
            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int sequence)
        {
            logger.LogInformation("=============================================");
            logger.LogInformation("id = " + id + " " + "sequence = "+ sequence);
            logger.LogInformation("=============================================");
            var result = await medicalRecordRespository.Delete(id, sequence);
            if(result == 0)
            {
                return BadRequest("Xóa hồ sơ thất bại");
            }
            else
            {
                return Ok("Xóa hồ sơ thành công");
            }
        }

       
        public async Task<IActionResult> Edit(int id, int sn)
        {
            ViewBag.service = new List<SelectListItem>()
            {
                new SelectListItem(){Value = "Khám tổng quát", Text = "Khám tổng quát"},
                new SelectListItem(){Value = "Nội soi", Text = "Nội soi"},
                new SelectListItem(){Value = "Siêu âm", Text = "Siêu âm"}
            };
            var record = await medicalRecordRespository.GetById(id, sn);
            EditMedicalRecordModel model = mapper.Map<EditMedicalRecordModel>(record);
            //get all dentist
            var allDentist = await dentistRepository.GetAllAsync();
            var dentistCreate = new List<SelectListItem>()
            {
                new SelectListItem() {Value = model.CreatedByDentistId, Text = "Không thay đổi"}
            };
            var dentistExam = new List<SelectListItem>()
            {
                new SelectListItem() {Value = model.ExamDentistId, Text = "Không thay đổi"}
            };
            foreach (var item in allDentist)
            {
                dentistCreate.Add(new SelectListItem() { Value = item.Id, Text = item.FullName });
                dentistExam.Add(new SelectListItem() { Value = item.Id, Text = item.FullName });
            }
            ViewBag.dentistCreate = dentistCreate;
            ViewBag.dentistExam = dentistExam;

            var customer = await customerRepository.GetAll();
            var selectCustomer = new List<SelectListItem>()
            {
                new SelectListItem(){Value = model.CustomerId, Text = "Không thay đổi"}
            };
            foreach (var item in customer)
            {
                selectCustomer.Add(new SelectListItem() { Value = item.Id, Text = item.FullName});
            }
            ViewBag.customer = selectCustomer;
			return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditMedicalRecordModel model, string oldDentistId)
        {
            if(ModelState.IsValid)
            {
                var res = await medicalRecordRespository.Edit(mapper.Map<MedicalRecord>(model));
                if (res == 1)
                {
                    TempData["Err"] = "Có lỗi xảy ra! Cập nhật hồ sơ thất bại";
                }
                else
                {
                    TempData["Success"] = "Cập nhật hồ sơ thành công";
                }
                return Redirect($"/MedicalRecord/Index?dentistId={oldDentistId}");
			}
            return View(model);
        }
     }
}
