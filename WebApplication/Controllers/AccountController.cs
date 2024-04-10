
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly CustomerRepository customerRepository;
		private readonly DentistRepository dentistRepository;
		private readonly EmployeeRepository employeeRepository;

		public AccountController(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,
            CustomerRepository customerRepository,
            DentistRepository dentistRepository,
            EmployeeRepository employeeRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.customerRepository = customerRepository;
			this.dentistRepository = dentistRepository;
			this.employeeRepository = employeeRepository;
		}
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser()
                {
                    UserName = model.PhoneNumber
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
					result = await userManager.AddToRoleAsync(user, "Customer");
					if (result.Succeeded)
                    {
						await signInManager.SignInAsync(user, isPersistent: false);
                        var customer = new Customer()
                        {
                            AccountId = user.Id,
                            PhoneNumber = model.PhoneNumber,
                            FullName = model.FullName,
                            Address = model.Address,
                            DayOfBirth = model.DayOfBirth
                        };
                        await customerRepository.AddCustomerAsync(customer);

                        return RedirectToAction("Index", "Home");
					}
					
					
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        public IActionResult Login(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
			return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                
                var result = await signInManager.PasswordSignInAsync(model.UserName, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.UserName);
                    if (user.IsLocked)
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa");
                        return View(model);
                    }
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                
                else
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không hợp lệ");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> IsAccountAvailable(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return Json(true);
            return Json($"Số điện thoại đã tồn tại");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "Customer, Employee, Dentist")]
        public async Task<IActionResult> EditProfile(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                Customer customer;
                Dentist dentist;
                Employee employee;
                EditProfileModel editProfile;

                if (await userManager.IsInRoleAsync(user, "Customer"))
                {
                    customer = await customerRepository.GetCustomerByAccountAsync(user);
                    editProfile = new EditProfileModel()
                    {
                        Id = user.Id,
                        FullName = customer.FullName,
                        DayOfBirth = customer.DayOfBirth,
                        Address = customer.Address
                    };
                    return View(editProfile);
                }
                else if (await userManager.IsInRoleAsync(user, "Dentist"))
                {
                    dentist = await dentistRepository.GetDentistByAccountAsync(user);
                    editProfile = new EditProfileModel()
                    {
                        Id = user.Id,
                        FullName = dentist.FullName
                    };
                    return View(editProfile);
                }
                else if (await userManager.IsInRoleAsync(user, "Employee"))
                {
                    employee = await employeeRepository.GetEmployeeByAccountAsync(user);
                    editProfile = new EditProfileModel()
                    {
                        Id = user.Id,
                        FullName = employee.FullName
                    };

                    return View(editProfile);
                }
            }
            return View();
  
        }
        [HttpPost]
        [Authorize(Roles = "Customer, Employee, Dentist")]
        public async Task<IActionResult> EditProfile(string id, EditProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);

                if (await userManager.IsInRoleAsync(user, "Customer"))
                {
                    Customer customer = await customerRepository.GetCustomerByAccountAsync(user);
                    customer.FullName = model.FullName ?? string.Empty;
                    customer.DayOfBirth = model.DayOfBirth.Value;
                    customer.Address = model.Address ?? string.Empty;
                    await customerRepository.UpdateCustomerAsync(customer);
                    return RedirectToAction("Index", "Home");
                }
                else if (await userManager.IsInRoleAsync(user, "Dentist"))
                {
                    Dentist dentist = await dentistRepository.GetDentistByAccountAsync(user);
                    dentist.FullName = model.FullName ?? string.Empty;
                    await dentistRepository.UpdateDentistAsync(dentist);
                    return RedirectToAction("Index", "Home");
                }
                else if (await userManager.IsInRoleAsync(user, "Employee"))
                {
                    Employee employee = await employeeRepository.GetEmployeeByAccountAsync(user);
                    employee.FullName = model.FullName ?? string.Empty;
                    await employeeRepository.UpdateEmployeeAsync(employee);
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
