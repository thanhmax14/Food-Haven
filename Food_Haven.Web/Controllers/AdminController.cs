using Microsoft.AspNetCore.Mvc;

namespace Food_Haven.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
