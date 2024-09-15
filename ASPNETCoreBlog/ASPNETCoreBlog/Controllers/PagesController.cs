using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.Controllers;

public class PagesController : Controller
{
    private readonly BlogSystemContext _context;

    public PagesController(BlogSystemContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = "PagesCache")]
    public async Task<IActionResult> Index(string slug)
    {
        try
        {
            // Check if the slug is null or empty
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            // Sanitize the slug to prevent XSS attacks
            var sanitizer = new HtmlSanitizer();
            var sanitizedSlug = sanitizer.Sanitize(slug);

            // Check if a page with the sanitized slug exists
            if (!await _context.Pages.AnyAsync(x => x.Slug == sanitizedSlug))
            {
                return NotFound();
            }

            // Retrieve the page including related sidebars
            var page = await _context.Pages
                .AsNoTracking()
                .Cacheable()
                .FirstOrDefaultAsync(x => x.Slug == sanitizedSlug);

            // Check if the page was found
            if (page == null)
            {
                return NotFound();
            }

            // Return the view with the page data
            return View(page);
        }
        catch (Exception ex)
        {
            return NotFound();
        }
    }
}