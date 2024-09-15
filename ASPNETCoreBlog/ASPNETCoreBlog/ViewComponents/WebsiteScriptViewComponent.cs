using Microsoft.AspNetCore.Mvc;

namespace CWACoreCMS.ViewComponents
{
    [ViewComponent(Name = "WebsiteScript")]
    public class WebsiteScriptViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}