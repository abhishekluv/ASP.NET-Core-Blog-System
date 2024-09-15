using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "CustomJS")]
    public class CustomJSViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public CustomJSViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var customCss = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => new CustomJSViewModel { IsCustomJSOn = x.IsCustomJSOn, CustomJS = x.CustomJavaScript }).Cacheable().FirstOrDefaultAsync();
            return View(customCss);
        }
    }
}
