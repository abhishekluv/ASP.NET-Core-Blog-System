using System.Linq.Dynamic.Core;
using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using ASPNETCoreBlog.Services;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class TagsController : BaseController
{
    private readonly BlogSystemContext _context;

    public TagsController(BlogSystemContext context)
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
    public async Task<JsonResult> GetTagsAsJson()
    {
        // server-side parameter
        var start = Convert.ToInt32(Request.Form["start"]);
        var length = Convert.ToInt32(Request.Form["length"]);
        string search = Request.Form["search[value]"];
        string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
        string sortDir = Request.Form["order[0][dir]"];

        if (!isAdmin) return Json(null);
        //_context.ChangeTracker.LazyLoadingEnabled = false;
        var tags = _context.Tags.OrderByDescending(x => x.Id).AsNoTracking().AsQueryable();

        var totalCount = tags.Count();

        if (!string.IsNullOrEmpty(search))
            tags = tags.Where(x => x.TagDescription.ToLower().Contains(search.ToLower()));

        tags = tags.OrderBy(sortColumnName + " " + sortDir);

        tags = tags.Skip(start).Take(length);

        var myTags = await tags.Select(x => new { x.Id, x.Name, x.TagDescription }).Cacheable().ToListAsync();

        var options = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return Json(new
        {
            data = myTags,
            draw = Convert.ToInt32(Request.Form["draw"]),
            recordsTotal = totalCount,
            recordsFiltered = totalCount
        }, options);
    }

    [HttpGet]
    public IActionResult CreateTag()
    {
        if (isAdmin) return View();
        return RedirectToAction("AccessDenied", "Account", new { area = "" });
    }

    [HttpPost]
    public async Task<ActionResult> CreateTag(Tag tag)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { area = "" });
        if (!ModelState.IsValid) return View(tag);
        var tagName = SlugHelper.Create(true, tag.Name); //generating proper slugs

        if (!_context.Tags.Any(x => x.Name == tagName))
        {
            tag.TagDescription = tag.TagDescription;
            tag.Name = tagName;
            tag.DatePublished = DateTime.UtcNow;

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", $"Tag {tag.Name} already exists");
        return View(tag);
    }

    [HttpGet]
    public async Task<IActionResult> EditTag(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var tag = await _context.Tags.SingleOrDefaultAsync(x => x.Id == id);
        return View(tag);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTag(Tag tag)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { area = "" });
        if (!ModelState.IsValid) return View(tag);
        var tagName = SlugHelper.Create(true, tag.Name);

        if (!_context.Tags.Where(x => x.Id != tag.Id).Any(x => x.Name == tagName))
        {
            var tagFromDb = await _context.Tags.Where(y => y.Id == tag.Id).FirstOrDefaultAsync();

            tagFromDb.TagDescription = tag.TagDescription;
            tagFromDb.Name = tagName;
            tagFromDb.TagSummary = tag.TagSummary;
            tagFromDb.TagImage = tag.TagImage;
            //tagFromDb.DatePublished = DateTime.UtcNow;

            _context.Tags.Update(tagFromDb);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", $"Tag {tag.Name} already exists");
        return View(tag);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteTag(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var tag = await _context.Tags.SingleOrDefaultAsync(x => x.Id == id);
        return View(tag);
    }

    [HttpPost]
    [ActionName("DeleteTag")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTagPost(int id)
    {
        var tag = await _context.Tags.SingleOrDefaultAsync(x => x.Id == id);

        try
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Cannot Delete Tag as it has a BlogPosts: {ex.InnerException.Message}");
            return View(tag);
        }

        return RedirectToAction("Index");
    }
}