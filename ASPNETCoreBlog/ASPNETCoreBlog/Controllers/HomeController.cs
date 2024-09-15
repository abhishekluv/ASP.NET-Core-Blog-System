using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogSystemContext _context;

        public HomeController(BlogSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "HomePageCache")]
        public async Task<IActionResult> Index()
        {
            // Fetch the home page from the database
            var homePage = await _context.HomePage
                .Where(x => x.Id == 1)
                .AsNoTracking()
                .Cacheable()
                .FirstOrDefaultAsync();

            // Check if homePage is null
            if (homePage == null)
            {
                return NotFound();
            }

            // Return the view with the homePage data
            return View(homePage);
        }
    }
}