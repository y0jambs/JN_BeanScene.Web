using Microsoft.AspNetCore.Mvc;

namespace BeanScene.Web.Controllers
{
    public class DrawingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
