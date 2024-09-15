using Microsoft.AspNetCore.Mvc;

namespace CWACoreCMS.ViewComponents
{
    [ViewComponent(Name = "WebsiteStyles")]
    public class WebsiteStylesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
