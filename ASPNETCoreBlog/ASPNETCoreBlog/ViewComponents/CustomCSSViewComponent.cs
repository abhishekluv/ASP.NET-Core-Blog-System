using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "CustomCSS")]
    public class CustomCSSViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public CustomCSSViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var customCss = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => new CustomCSSViewModel { IsCustomCSSOn = x.IsCustomCSSOn, CustomCSS = x.CustomCSS }).Cacheable().FirstOrDefaultAsync();
            return View(customCss);
        }
    }
}