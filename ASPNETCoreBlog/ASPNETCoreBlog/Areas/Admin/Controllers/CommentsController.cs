using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace CWACoreCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommentsController : BaseController
    {
        private readonly BlogSystemContext _context;

        public CommentsController(BlogSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (isAdmin) return View();
            return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        }

        [HttpPost]
        [OutputCache(Duration = 10)]
        public async Task<JsonResult> GetCommentsAsJson()
        {
            // server-side parameter
            var start = Convert.ToInt32(Request.Form["start"]);
            var length = Convert.ToInt32(Request.Form["length"]);
            string search = Request.Form["search[value]"];
            string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
            string sortDir = Request.Form["order[0][dir]"];

            if (!isAdmin) return Json(null);
            //_context.ChangeTracker.LazyLoadingEnabled = false;
            var comments = _context.Comments.Include(x => x.BlogPost).OrderByDescending(x => x.Id).AsNoTracking()
                .AsQueryable();

            var totalCount = comments.Count();

            if (!string.IsNullOrEmpty(search))
                comments = comments.Where(x => x.Name.ToLower().Contains(search.ToLower()));

            comments = comments.OrderBy(sortColumnName + " " + sortDir);

            comments = comments.Skip(start).Take(length);

            var myComments = await comments.Select(x => new
            { x.Id, x.Name, x.IsApproved, x.CommentMessage, BlogPost = x.BlogPost.Title }).Cacheable().ToListAsync();

            var options = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return Json(new
            {
                data = myComments,
                draw = Convert.ToInt32(Request.Form["draw"]),
                recordsTotal = totalCount,
                recordsFiltered = totalCount
            }, options);
        }

        [HttpPost]
        public async Task<JsonResult> ApproveComment(int id)
        {
            if (!isAdmin) return Json(null);
            var comment = await _context.Comments.FindAsync(id);

            if (comment.IsApproved.Value)
                return Json(new { Message = $"Comment by {comment.Name} is already Approved" });

            comment.IsApproved = true;

            await _context.SaveChangesAsync();

            return Json(new { Message = $"Comment by {comment.Name} is now Approved" });
        }

        [HttpPost]
        public async Task<JsonResult> DisApproveComment(int id)
        {
            if (!isAdmin) return Json(null);
            var comment = await _context.Comments.FindAsync(id);

            if (!comment.IsApproved.Value)
                return Json(new { Message = $"Comment by {comment.Name} is already Dis-Approved" });
            comment.IsApproved = false;

            await _context.SaveChangesAsync();

            return Json(new { Message = $"Comment by {comment.Name} is now Dis-Approved" });
        }

        [HttpGet]
        public async Task<IActionResult> EditComment(int id)
        {
            if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });

            var comment = await _context.Comments.FindAsync(id);
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong..");
                return View(comment);
            }

            if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });

            if (comment != null)
            {
                var commentFromDb = _context.Comments.FirstOrDefault(c => c.Id == comment.Id);

                commentFromDb.Name = comment.Name;
                commentFromDb.EmailAddress = comment.EmailAddress;
                commentFromDb.CommentMessage = comment.CommentMessage;

                _context.Comments.Update(commentFromDb);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Something went wrong..");
            return View(comment);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });

            var comment = await _context.Comments.FindAsync(id);
            return View(comment);
        }

        [HttpPost]
        [ActionName("DeleteComment")]
        public async Task<IActionResult> DeleteCommentPost(int id)
        {
            if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });

            var comment = await _context.Comments.FindAsync(id);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}