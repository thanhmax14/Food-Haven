using Microsoft.AspNetCore.Mvc;

namespace Food_Haven.Web.Controllers
{
    public class SellerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
