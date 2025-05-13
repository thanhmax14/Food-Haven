using Microsoft.AspNetCore.Mvc;

namespace Food_Haven.Web.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
