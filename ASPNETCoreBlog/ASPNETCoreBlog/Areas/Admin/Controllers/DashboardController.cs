using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DashboardController : BaseController
    {
        private readonly BlogSystemContext _context;

        public DashboardController(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!isAdmin)
                return RedirectToAction("AccessDenied", "Account", new { Area = "" });

            // Fetch contacts, reviews, and comments asynchronously in parallel
            var contactsTask = _context.Contacts.Cacheable().ToListAsync();
            var commentsTask = _context.Comments.Cacheable().ToListAsync();

            await Task.WhenAll(contactsTask, commentsTask);

            var contacts = contactsTask.Result;
            var comments = commentsTask.Result;

            // Initialize ViewModel with aggregated data
            var viewModel = new DashboardViewModel
            {
                ContactCount = contacts.Count,
                CommentCount = comments.Count,
                CommentApprovedCount = comments.Count(x => x.IsApproved == true),
                Top10Contacts = contacts.OrderByDescending(x => x.Id).Take(10).ToList(),
                Top10Comments = comments.OrderByDescending(x => x.Id).Take(10).ToList()
            };

            return View(viewModel);
        }

    }
}
