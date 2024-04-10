using AutoMapper;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repositories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
		private readonly UserManager<AppUser> userManager;
        private readonly DentistRepository dentistRepository;
        private readonly EmployeeRepository employeeRepository;
        private readonly CustomerRepository customerRepository;
        private readonly IMapper mapper;

        public AdministrationController(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            DentistRepository dentistRepository,
            EmployeeRepository employeeRepository,
            CustomerRepository customerRepository,
            IMapper mapper)
        {
            this.roleManager = roleManager;
			this.userManager = userManager;
            this.dentistRepository = dentistRepository;
            this.employeeRepository = employeeRepository;
            this.customerRepository = customerRepository;
            this.mapper = mapper;
        }

        #region Features concerning user
        public async Task<IActionResult> ListUsers()
        {
            var users = await userManager.Users.ToListAsync();
            List<ListUsersModel> userList = new List<ListUsersModel>();
            foreach (var user in users)
            {
                if (!(await userManager.IsInRoleAsync(user, "Customer"))
                    && !(await userManager.IsInRoleAsync(user, "Admin")))
                {
                    ListUsersModel model = new ListUsersModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        IsLocked = user.IsLocked
                    };
                   
                    if (await userManager.IsInRoleAsync(user, "Dentist"))
                    {
                        var dentist = await dentistRepository.GetDentistByAccountAsync(user);
						model.FullName = dentist.FullName;
						model.PhoneNumber = dentist.PhoneNumber;
                        model.Role = "Dentist";
                    }
                    else if (await userManager.IsInRoleAsync(user, "Employee"))
					{
						var employee = await employeeRepository.GetEmployeeByAccountAsync(user);
						model.FullName = employee.FullName;
						model.PhoneNumber = employee.PhoneNumber;
						model.Role = "Employee";
					}
                    userList.Add(model);
				}
                else if (await userManager.IsInRoleAsync(user, "Customer"))
                {
                    ListUsersModel model = new ListUsersModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        IsLocked = user.IsLocked
                    };
                    var customer = await customerRepository.GetCustomerByAccountAsync(user);
                    model.FullName = customer.FullName;
                    model.PhoneNumber = customer.PhoneNumber;
                    model.Role = "Customer";
					userList.Add(model);
				}
            }
            return View(userList);
        }
        public IActionResult CreateUser()
        {
            ViewData["UserTypeList"] = new List<SelectListItem>()
            {
                new SelectListItem("Nha sĩ", "Dentist"),
                new SelectListItem("Nhân viên", "Employee")
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.UserName);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, $"User with user name {user.UserName} already exists");
                    return View(model);
                }
                var newUser = new AppUser()
                {
                    UserName = model.UserName
                };
                var result = await userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    if (model.UserType == "Dentist")
                    {
                        result = await userManager.AddToRoleAsync(newUser, "Dentist");
                        if (result.Succeeded)
                        {
                            var dentist = new Dentist()
                            {
                                AccountId = newUser.Id,
                                PhoneNumber = model.PhoneNumber,
                                FullName = model.FullName,
                                Account = newUser
                            };
                            await dentistRepository.AddDentistAsync(dentist);

                            return RedirectToAction("ListUsers", "Administration");
                        }
                    }
                    else if (model.UserType == "Employee")
                    {
                        result = await userManager.AddToRoleAsync(newUser, "Employee");
                        if (result.Succeeded)
                        {
                            var employee = new Employee()
                            {
                                AccountId = newUser.Id,
                                PhoneNumber = model.PhoneNumber,
                                FullName = model.FullName,
                                Account = newUser
                            };
                            await employeeRepository.AddEmployeeAsync(employee);

                            return RedirectToAction("ListUsers", "Administration");
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View();
            }
            var roles = await userManager.GetRolesAsync(user);
            EditUserModel model = null;
            if (await userManager.IsInRoleAsync(user, "Dentist"))
            {
                Dentist dentist = await dentistRepository.GetDentistByAccountAsync(user);
                model = new EditUserModel()
                {
                    Id = dentist.Id,
                    UserName = user.UserName,
                    PhoneNumber = dentist.PhoneNumber,
                    FullName = dentist.FullName
                };
            }
            else if (await userManager.IsInRoleAsync(user, "Employee"))
            {
                Employee employee = await employeeRepository.GetEmployeeByAccountAsync(user);
                model = new EditUserModel()
                {
                    Id = employee.Id,
                    UserName = user.UserName,
                    PhoneNumber = employee.PhoneNumber,
					FullName = employee.FullName
				};
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, $"Role with Id = {model.Id} is not exist");
                    return View("Error");
                }

                // Update phone number
                if (await userManager.IsInRoleAsync(user, "Dentist"))
                {
                    Dentist dentist = await dentistRepository.GetDentistByAccountAsync(user);
                    dentist.PhoneNumber = model.PhoneNumber;
                    dentist.FullName = model.FullName;
                    await dentistRepository.UpdateDentistAsync(dentist);
                }
                else if (await userManager.IsInRoleAsync(user, "Employee"))
                {
                    Employee employee = await employeeRepository.GetEmployeeByAccountAsync(user);
                    employee.PhoneNumber = model.PhoneNumber;
                    employee.FullName = model.FullName;
                    await employeeRepository.UpdateEmployeeAsync(employee);
                }


                user.UserName = model.UserName;
                
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Administration");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
			var user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound("Some errors occur");
			}
            

            if (await userManager.IsInRoleAsync(user, "Dentist"))
            {
                Dentist dentist = await dentistRepository.GetDentistByAccountAsync(user);
                await dentistRepository.DeleteDentistAsync(dentist);
            }
            else if (await userManager.IsInRoleAsync(user, "Employee"))
            {
                Employee employee = await employeeRepository.GetEmployeeByAccountAsync(user);
                await employeeRepository.DeleteEmployeeAsync(employee);
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok("Deleted successfully");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest("Some errors occur");


        }

        [HttpPost]
		public async Task<IActionResult> LockOrUnlockUser(string id, bool active)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (active)
            {
                user.IsLocked = false;
            }
            else
            {
                user.IsLocked = true;
            }
            await userManager.UpdateAsync(user);
            return Ok();
        }

        public async Task<IActionResult> ViewCustomerDetail(string customerName)
        {
            var res = await customerRepository.GetCustomerByNameAsync(customerName);

            var customer = res.Item1;
            var timthay = res.Item2;
			if (timthay == 1)
			{
				ViewBag.Notification = "Tìm thấy khách hàng";
			}
			if (customer == null)
            {
                return View(
                new CustomerModel()
                {
                    FullName = "",
                    PhoneNumber = "",
                    DayOfBirth = new DateTime(0),
                    Address = ""
                });
            }
            
            var model = mapper.Map<CustomerModel>(customer);
            return View(model);
        }
		#endregion
	}
}
