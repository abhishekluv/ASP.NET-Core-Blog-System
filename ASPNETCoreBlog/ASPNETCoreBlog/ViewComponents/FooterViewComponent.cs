using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "SiteFooter")]
    public class FooterViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public FooterViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var siteFooter = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => x.SiteFooter).Cacheable().FirstOrDefaultAsync();
            return View(model: siteFooter);
        }
    }
}
