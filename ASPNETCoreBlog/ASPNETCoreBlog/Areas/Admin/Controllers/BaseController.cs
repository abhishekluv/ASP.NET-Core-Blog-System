using ASPNETCoreBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASPNETCoreBlog.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected bool isAdmin;
        protected bool isAuthenticated;
        protected string? userName;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            isAdmin = context.HttpContext.IsAdmin();
            userName = context.HttpContext.User.Identity.Name;
            base.OnActionExecuting(context);
        }
    }
}
