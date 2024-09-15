using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "SiteName")]
    public class SiteNameViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public SiteNameViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var siteName = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => x.SiteName).Cacheable().FirstOrDefaultAsync();
            return View(model: siteName);
        }
    }
}
