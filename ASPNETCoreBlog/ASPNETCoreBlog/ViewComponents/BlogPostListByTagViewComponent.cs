using ASPNETCoreBlog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "BlogPostListByTag")]
    public class BlogPostListByTagViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public BlogPostListByTagViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tagName)
        {
            var tag = await _context.Tags.Include(x => x.Posts.Where(x => x.IsPostEnabled && x.IsVisibleToSearchEngine).OrderBy(x => x.Id)).Where(x => x.Name == tagName).FirstOrDefaultAsync();
            return View(tag);
        }
    }
}
