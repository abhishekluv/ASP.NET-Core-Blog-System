using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "DNSPrefetch")]
    public class DNSPrefetchViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public DNSPrefetchViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dnsPrefetch = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => new DNSPrefetchViewModel { DNSPreconnect = x.DNSPreconnect, DNSPrefetch = x.DNSPrefetch }).Cacheable().FirstOrDefaultAsync();
            return View(dnsPrefetch);
        }
    }
}
