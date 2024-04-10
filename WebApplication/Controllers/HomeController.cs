using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
	public class HomeController : Controller
    {

        public HomeController()
        {
            
        }
        public IActionResult Index()
        {
            var notifi = TempData["Err"];
            return View();
        }



    }
}
