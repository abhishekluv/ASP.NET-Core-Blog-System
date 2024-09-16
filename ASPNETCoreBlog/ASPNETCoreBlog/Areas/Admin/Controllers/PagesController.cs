using System.Linq.Dynamic.Core;
using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using ASPNETCoreBlog.Services;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class PagesController : BaseController
{
    private readonly BlogSystemContext _context;

    public PagesController(BlogSystemContext context)
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
    public async Task<JsonResult> GetPagesAsJson()
    {
        // server-side parameter
        var start = Convert.ToInt32(Request.Form["start"]);
        var length = Convert.ToInt32(Request.Form["length"]);
        string search = Request.Form["search[value]"];
        string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
        string sortDir = Request.Form["order[0][dir]"];

        if (!isAdmin) return Json(null);
        //_context.ChangeTracker.LazyLoadingEnabled = false;
        var pages = _context.Pages.OrderByDescending(x => x.Id).AsNoTracking()
            .AsQueryable();

        var totalCount = pages.Count();

        if (!string.IsNullOrEmpty(search))
            pages = pages.Where(x => x.Title.ToLower().Contains(search.ToLower()));

        pages = pages.OrderBy(sortColumnName + " " + sortDir);

        pages = pages.Skip(start).Take(length);

        var myPages =
            await pages.Select(x => new { x.Id, x.Title, x.Slug }).Cacheable().ToListAsync();

        var options = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return Json(new
        {
            data = myPages,
            draw = Convert.ToInt32(Request.Form["draw"]),
            recordsTotal = totalCount,
            recordsFiltered = totalCount
        }, options);
    }

    [HttpGet]
    public IActionResult CreatePage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePage(PageViewModel viewModel)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        if (ModelState.IsValid)
        {
            var slug = SlugHelper.Create(true, string.IsNullOrEmpty(viewModel.Slug) ? viewModel.Title : viewModel.Slug);

            //checking slug exists
            if (_context.Pages.Any(x => x.Slug == slug))
            {
                ModelState.AddModelError("", "Slug already exists");
                return View(viewModel);
            }

            var page = new Page
            {
                Slug = slug,
                Title = viewModel.Title,
                AfterContent = viewModel.AfterContent,
                Content = viewModel.Content,
                BeforeContent = viewModel.BeforeContent,
                CallToAction = viewModel.CallToAction,
                FooterContent = viewModel.FooterContent,
                MetaDescription = viewModel.MetaDescription,
                MetaKeywords = viewModel.MetaKeywords,
                Image = viewModel.Image,
                IsVisibleToSearchEngine = viewModel.IsVisibleToSearchEngine
            };

            //trainingPage.DatePublished = DateTime.Now;
            //trainingPage.DateUpdated = DateTime.Now;


            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(viewModel);
    }

    //edit
    [HttpGet]
    public async Task<IActionResult> EditPage(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var viewModel = await _context.Pages.Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        var page = new PageViewModel
        {
            Id = id,
            Slug = viewModel.Slug,
            Title = viewModel.Title,
            AfterContent = viewModel.AfterContent,
            Content = viewModel.Content,
            BeforeContent = viewModel.BeforeContent,
            CallToAction = viewModel.CallToAction,
            FooterContent = viewModel.FooterContent,
            MetaDescription = viewModel.MetaDescription,
            MetaKeywords = viewModel.MetaKeywords,
            Image = viewModel.Image,
            IsVisibleToSearchEngine = viewModel.IsVisibleToSearchEngine,
        };

        return View(page);
    }

    [HttpPost]
    public async Task<IActionResult> EditPage(PageViewModel viewModel)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        if (ModelState.IsValid)
        {
            var slug = SlugHelper.Create(true, string.IsNullOrEmpty(viewModel.Slug) ? viewModel.Title : viewModel.Slug);

            //checking slug exists
            if (_context.Pages.Where(x => x.Id != viewModel.Id).Any(x => x.Slug == slug))
            {
                ModelState.AddModelError("", "Slug already exists");
                return View(viewModel);
            }

            var page = await _context.Pages.Where(x => x.Id == viewModel.Id)
                .FirstOrDefaultAsync();

            if (page != null)
            {
                page.Slug = slug;
                page.Title = viewModel.Title;
                page.AfterContent = viewModel.AfterContent;
                page.Content = viewModel.Content;
                page.BeforeContent = viewModel.BeforeContent;
                page.CallToAction = viewModel.CallToAction;
                page.FooterContent = viewModel.FooterContent;
                page.MetaDescription = viewModel.MetaDescription;
                page.MetaKeywords = viewModel.MetaKeywords;
                page.Image = viewModel.Image;
                //trainingPage.DateUpdated = DateTime.Now;

                _context.Pages.Update(page);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> DeletePage(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var page = await _context.Pages.Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        return View(page);
    }


    [HttpPost]
    [ActionName("DeletePage")]
    public async Task<IActionResult> DeletePagePost(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var page = await _context.Pages.Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        _context.Pages.Remove(page);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}