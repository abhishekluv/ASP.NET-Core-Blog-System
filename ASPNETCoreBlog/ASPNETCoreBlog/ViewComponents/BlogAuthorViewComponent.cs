using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    public class BlogAuthorViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public BlogAuthorViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogAuthor = await _context.SiteSettings.Where(x => x.Id == 1).Select(x => x.BlogAuthorSummary).Cacheable().FirstOrDefaultAsync();
            return View(model: blogAuthor);
        }
    }
}