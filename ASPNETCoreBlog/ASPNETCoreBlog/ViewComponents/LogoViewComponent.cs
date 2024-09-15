using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    public class LogoViewComponent :ViewComponent
    {
        private readonly BlogSystemContext _context;

        public LogoViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var siteLogo = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => x.LogoURL).Cacheable().FirstOrDefaultAsync();
            return View(model: siteLogo);
        }
    }
}
