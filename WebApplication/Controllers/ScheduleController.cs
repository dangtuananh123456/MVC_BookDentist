using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;
using System.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
	public class ScheduleController : Controller
	{
		private readonly DentistRepository dentistRepository;
        private readonly AppointmentScheduleRepository appointmentScheduleRepository;

        public ScheduleController(DentistRepository dentistRepository,
            AppointmentScheduleRepository appointmentScheduleRepository)
        {
			this.dentistRepository = dentistRepository;
            this.appointmentScheduleRepository = appointmentScheduleRepository;
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAvailableDentists(DateTime date, DateTime startTime, DateTime endTime)
		{
            DateTime sTime = new DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    startTime.Hour,
                    startTime.Minute,
                    startTime.Second
                );
            DateTime eTime = new DateTime(
                date.Year,
                date.Month,
                date.Day,
                endTime.Hour,
                endTime.Minute,
                endTime.Second
            );
            var dentists = await dentistRepository.GetAllAsync();
			var availableDentists = new List<Dentist>();
            foreach (var dentist in dentists)
			{
				bool isAvailable = true;
                var schedules = await appointmentScheduleRepository.GetAppointmentsOfDentist(dentist.Id);
				if (schedules != null)
				{
					foreach (var schedule in schedules)
					{
						if ((sTime >= schedule.StartTime && sTime <= schedule.EndTime)
							|| (eTime >= schedule.StartTime && eTime <= schedule.EndTime))
						{
							isAvailable = false;
							break;
						}
					}
				}
				
				if (isAvailable)
				{
					availableDentists.Add(dentist);
				}
            }
            return Ok(availableDentists);
		}
		[Authorize(Roles = "Customer")]
        public IActionResult MakeAppointment(string customerId)
		{
			ViewData["CustomerId"] = customerId;
			
            return View();
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
		public async Task<IActionResult> MakeAppointment(CreateAppointmentModel model)
		{
			if (ModelState.IsValid)
			{
				var appointment = new AppointmentSchedule()
				{
					DentistId = model.DentistId,
					CustomerId = model.CustomerId
				};
				DateTime sTime = new DateTime(
					model.Date.Year,
					model.Date.Month,
					model.Date.Day,
					model.StartTime.Hour,
					model.StartTime.Minute,
					model.StartTime.Second
				);
                DateTime eTime = new DateTime(
                    model.Date.Year,
                    model.Date.Month,
                    model.Date.Day,
                    model.EndTime.Hour,
                    model.EndTime.Minute,
                    model.EndTime.Second
                );

				appointment.StartTime = sTime;
				appointment.EndTime = eTime;

                var res = await appointmentScheduleRepository.AddAppoinment(appointment);
				if (res != 0)
				{
					TempData["Err"] = "Có lỗi xảy ra. Thêm lịch hẹn thất bại";
					
				}
				else
				{
					TempData["Success"] = "Thêm lịch hẹn thành công";
				}
				return RedirectToAction("GetCustomerSchedules", new { customerId = appointment.CustomerId });
			}
            
            return View("ReviewAppointment", model);
		}
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> ReviewAppointment(CreateAppointmentModel model)
        {
			var dentist = await dentistRepository.GetDentistByIdAsync(model.DentistId);

            ViewData["DentistName"] = dentist.FullName;
            return View(model);
        }

		[Authorize(Roles = "Dentist")]
		[HttpGet]
		public async Task<IActionResult> GetAppointmentsByDate(string dentistId, DateTime date)
		{
			try
			{
				ViewData["ScheduleDate"] = date;
                var scheduleList = await appointmentScheduleRepository
                .GetAppointmentsOfDentist(dentistId);
				var list = scheduleList
					.Where(x =>
					x.StartTime.Year == date.Year
					&& x.StartTime.Month == date.Month
					&& x.StartTime.Day == date.Day).ToList();

                return PartialView("_ListSchedulePartial", list);

			}
			catch (Exception)
			{
				return BadRequest();
			}
		}

		[Authorize(Roles = "Dentist")]
        [HttpGet]
        public IActionResult ListAppointmentSchedules(string dentistId)
        {
			ViewData["DentistId"] = dentistId;
			return View();
        }

		[Authorize(Roles = "Dentist")]
		[HttpGet]
		public IActionResult CreatePersonalSchedule(string dentistId)
		{
			ViewData["DentistId"] = dentistId;

            return View();
		}

        [Authorize(Roles = "Dentist")]
        [HttpPost]
        public async Task<IActionResult> CreatePersonalSchedule(PersonalScheduleModel model)
        {
            if (ModelState.IsValid)
			{
				var schedule = new AppointmentSchedule()
				{
					StartTime = model.StartTime,
					EndTime = model.EndTime,
					DentistId = model.DentistId
				};
				await appointmentScheduleRepository.AddAppoinment(schedule);
				return RedirectToAction("ListAppointmentSchedules", new { dentistId = model.DentistId});
			}

            return View(model);
        }
		[Authorize(Roles = "Dentist")]
		public async Task<IActionResult> EditPersonalSchedule(string dentistId, string startTimeStr)
		{
			ViewData["DentistId"] = dentistId;

            if (!DateTime.TryParse(startTimeStr, out DateTime startTime))
			{
				return BadRequest("Invalid startTime format");
			}

			var personalSchedule = await appointmentScheduleRepository.FindAsync(dentistId, startTime);
			if (personalSchedule == null)
			{
				return BadRequest();
			}

			var model = new PersonalScheduleModel()
			{
				Date = personalSchedule.StartTime,
				StartTime = personalSchedule.StartTime,
				EndTime = personalSchedule.EndTime,
				DentistId = personalSchedule.DentistId
			};
			ViewData["OldStartTime"] = startTime.ToString();
			return View(model);
		}

		[Authorize(Roles = "Dentist")]
		[HttpPost]
		public async Task<IActionResult> EditPersonalSchedule(PersonalScheduleModel model, string oldStartTimeStr)
		{
			if (ModelState.IsValid)
			{
                if (!DateTime.TryParse(oldStartTimeStr, out DateTime oldStartTime))
                {
                    return BadRequest("Invalid oldStartTime format");
                }

                var schedule = await appointmentScheduleRepository
					.FindAsync(model.DentistId, oldStartTime);
				if (schedule == null) return BadRequest();

				DateTime sTime = new DateTime(
					model.Date.Year,
					model.Date.Month,
					model.Date.Day,
					model.StartTime.Hour,
					model.StartTime.Minute,
					model.StartTime.Second
				);
				DateTime eTime = new DateTime(
					model.Date.Year,
					model.Date.Month,
					model.Date.Day,
					model.EndTime.Hour,
					model.EndTime.Minute,
					model.EndTime.Second
				);
				await appointmentScheduleRepository.UpdateAsync(schedule, sTime, eTime);
				return RedirectToAction("ListAppointmentSchedules", new { dentistId = model.DentistId });
			}
			return View(model);
		}

		#region Schedules of customer
		[Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerSchedules(string customerId)
		{
			var schedules = await appointmentScheduleRepository.GetAppointmentsOfCustomer(customerId);
			return View(schedules);
        }

		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> EditSchedule(string dentistId, string customerId, string startTimeStr)
		{
			ViewData["CustomerId"] = customerId;
			

			if (!DateTime.TryParse(startTimeStr, out DateTime startTime))
			{
				return BadRequest("Invalid startTime format");
			}

			var schedule = await appointmentScheduleRepository.FindAsync(dentistId, startTime);
			if (schedule == null)
			{
				return BadRequest();
			}

			var model = new ScheduleModel()
			{
				Date = schedule.StartTime,
				StartTime = schedule.StartTime,
				EndTime = schedule.EndTime,
				DentistId = schedule.DentistId
			};

			var dentistList = await dentistRepository.GetAllAsync();
			var selectListDentists = new List<SelectListItem>();
			var selectedDentist = await dentistRepository.GetDentistByIdAsync(dentistId);
			selectListDentists.Add(new SelectListItem(selectedDentist.FullName, dentistId));

            foreach (var dentist in dentistList)
			{
				if (dentist.Id != dentistId)
				{
                    selectListDentists.Add(new SelectListItem(dentist.FullName, dentist.Id));
                }
            }
			ViewData["DentistList"] = selectListDentists;


            ViewData["OldStartTime"] = startTime.ToString();
            ViewData["OldDentistId"] = dentistId;
			return View(model);
		}

		[Authorize(Roles = "Customer")]
		[HttpPost]
		public async Task<IActionResult> EditSchedule(ScheduleModel model, string oldDentistId, string oldStartTimeStr)
		{
			if (ModelState.IsValid)
			{
				if (!DateTime.TryParse(oldStartTimeStr, out DateTime oldStartTime))
				{
					return BadRequest("Invalid oldStartTime format");
				}

				var schedule = await appointmentScheduleRepository
					.FindAsync(oldDentistId, oldStartTime);
				if (schedule == null) return BadRequest();

				DateTime sTime = new DateTime(
					model.Date.Year,
					model.Date.Month,
					model.Date.Day,
					model.StartTime.Hour,
					model.StartTime.Minute,
					model.StartTime.Second
				);
				DateTime eTime = new DateTime(
					model.Date.Year,
					model.Date.Month,
					model.Date.Day,
					model.EndTime.Hour,
					model.EndTime.Minute,
					model.EndTime.Second
				);
				await appointmentScheduleRepository.UpdateAsync(schedule, sTime, eTime, model.DentistId);
				return RedirectToAction("GetCustomerSchedules", new { customerId = schedule.CustomerId});
			}
			return View(model);
		}
		#endregion
	}
}
