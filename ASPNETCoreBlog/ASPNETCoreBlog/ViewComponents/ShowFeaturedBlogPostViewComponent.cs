using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{

    [ViewComponent(Name = "ShowFeaturedBlogPost")]
    public class ShowFeaturedBlogPostViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public ShowFeaturedBlogPostViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogposts = await _context.BlogPosts.Include(x => x.Comments.Where(x => x.IsApproved.Value)).Where(x => x.IsFeatured).OrderByDescending(x => x.DatePublished).Cacheable().ToListAsync();
            return View(blogposts);
        }
    }
}
