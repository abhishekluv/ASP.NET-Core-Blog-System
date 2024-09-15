using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "Favicon")]
    public class FaviconViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public FaviconViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var favicon = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => x.FaviconURL).Cacheable().FirstOrDefaultAsync();
            return View(model: favicon);
        }
    }
}