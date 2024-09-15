using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreBlog.Controllers
{
    public class ErrorController : Controller
    {

        [Route("Error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("404"); // Return the custom 404 view
                case 500:
                    return View("500"); // Return the custom 500 view
                default:
                    return View("404"); // Default error view
            }
        }

        [HttpGet]
        [Route("500")]
        public IActionResult AppError()
        {
            return View("~/Views/Shared/Error500.cshtml");
        }
    }
}