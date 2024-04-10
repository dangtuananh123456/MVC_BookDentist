using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Repositories;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class CustomerCreditController : Controller
    {
        private CreditRepository creditRepository;
        
        public CustomerCreditController(CreditRepository creditRepository)
        {
            this.creditRepository = creditRepository;
        }

        public IActionResult Index(string customerId)
        {
            ViewBag.id = customerId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AddMoneyCreditModel model)
        {
            if(ModelState.IsValid)
            {
                await creditRepository.AddMoney(model.CustomerId, model.Money);
                return Redirect("/");
            }
            return View();
        }
    }
}
