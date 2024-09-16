using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using ASPNETCoreBlog.Services;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class BlogPostsController : BaseController
{
    private readonly BlogSystemContext _context;

    public BlogPostsController(BlogSystemContext context)
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
    public async Task<JsonResult> GetBlogPostsAsJson()
    {
        // server-side parameter
        var start = Convert.ToInt32(Request.Form["start"]);
        var length = Convert.ToInt32(Request.Form["length"]);
        string search = Request.Form["search[value]"];
        string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
        string sortDir = Request.Form["order[0][dir]"];

        if (!isAdmin) return Json(null);
        //_context.ChangeTracker.LazyLoadingEnabled = false;
        var blogPosts = _context.BlogPosts.OrderByDescending(x => x.Id).AsSplitQuery().AsNoTracking()
            .AsQueryable();

        var totalCount = blogPosts.Count();

        if (!string.IsNullOrEmpty(search))
            blogPosts = blogPosts.Where(x => x.Title.ToLower().Contains(search.ToLower()));

        blogPosts = blogPosts.OrderBy(sortColumnName + " " + sortDir);

        blogPosts = blogPosts.Skip(start).Take(length);

        var myBlogPosts =
            await blogPosts.Select(x => new { x.Id, x.Title, x.Slug }).Cacheable().ToListAsync();

        var options = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return Json(new
        {
            data = myBlogPosts,
            draw = Convert.ToInt32(Request.Form["draw"]),
            recordsTotal = totalCount,
            recordsFiltered = totalCount
        }, options);
    }

    [HttpGet]
    public IActionResult CreateBlogPost()
    {
        ViewBag.Tags = GetTags();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlogPost(BlogPostViewModel viewModel)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        if (ModelState.IsValid)
        {
            var slug = SlugHelper.Create(true, string.IsNullOrEmpty(viewModel.Slug) ? viewModel.Title : viewModel.Slug);

            //checking slug exists
            if (_context.BlogPosts.Any(x => x.Slug == slug))
            {
                ModelState.AddModelError("", "Slug already exists");
                ViewBag.Tags = GetTags();
                return View(viewModel);
            }

            var blogPost = new BlogPost
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
                IsVisibleToSearchEngine = viewModel.IsVisibleToSearchEngine,
                IsFeatured = viewModel.IsFeatured,
                Ad1 = viewModel.Ad1,
                Ad2 = viewModel.Ad2,
                Ad3 = viewModel.Ad3,
                IsPostEnabled = viewModel.IsPostEnabled,
                BuyMeCoffee = viewModel.BuyMeCoffee,
                IsCommentEnabled = viewModel.IsCommentEnabled,
                DatePublished = viewModel.DatePublished,
                DateUpdated = viewModel.DatePublished
            };

            foreach (var tagId in viewModel.TagIds)
            {
                var tag = _context.Tags.Where(x => x.Id == tagId).FirstOrDefault();
                blogPost.Tags.Add(tag);
            }

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        ViewBag.Tags = GetTags();
        return View(viewModel);
    }

    //edit
    [HttpGet]
    public async Task<IActionResult> EditBlogPost(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var blogPost = await _context.BlogPosts.Include(x => x.Tags).Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        var viewModel = new BlogPostViewModel
        {
            Id = id,
            Slug = blogPost.Slug,
            Title = blogPost.Title,
            AfterContent = blogPost.AfterContent,
            Content = blogPost.Content,
            BeforeContent = blogPost.BeforeContent,
            CallToAction = blogPost.CallToAction,
            FooterContent = blogPost.FooterContent,
            MetaDescription = blogPost.MetaDescription,
            MetaKeywords = blogPost.MetaKeywords,
            Image = blogPost.Image,
            IsVisibleToSearchEngine = blogPost.IsVisibleToSearchEngine,
            IsFeatured = blogPost.IsFeatured,
            IsCommentEnabled = blogPost.IsCommentEnabled,
            Ad1 = blogPost.Ad1,
            Ad2 = blogPost.Ad2,
            Ad3 = blogPost.Ad3,
            IsPostEnabled = blogPost.IsPostEnabled,
            BuyMeCoffee = blogPost.BuyMeCoffee,
            TagIds = blogPost.Tags.Select(x => x.Id).ToArray()
        };

        ViewBag.Tags = GetTags();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditBlogPost(BlogPostViewModel viewModel)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        if (ModelState.IsValid)
        {
            var slug = SlugHelper.Create(true, string.IsNullOrEmpty(viewModel.Slug) ? viewModel.Title : viewModel.Slug);

            //checking slug exists
            if (_context.BlogPosts.Where(x => x.Id != viewModel.Id).Any(x => x.Slug == slug))
            {
                ModelState.AddModelError("", "Slug already exists");
                ViewBag.Tags = GetTags();
                return View(viewModel);
            }

            var blogPost = await _context.BlogPosts.Include(x => x.Tags).Where(x => x.Id == viewModel.Id).FirstOrDefaultAsync();

            blogPost.Slug = slug;
            blogPost.Title = viewModel.Title;
            blogPost.AfterContent = viewModel.AfterContent;
            blogPost.Content = viewModel.Content;
            blogPost.BeforeContent = viewModel.BeforeContent;
            blogPost.CallToAction = viewModel.CallToAction;
            blogPost.FooterContent = viewModel.FooterContent;
            blogPost.MetaDescription = viewModel.MetaDescription;
            blogPost.MetaKeywords = viewModel.MetaKeywords;
            blogPost.Image = viewModel.Image;
            blogPost.IsVisibleToSearchEngine = viewModel.IsVisibleToSearchEngine;
            blogPost.IsFeatured = viewModel.IsFeatured;
            blogPost.Ad1 = viewModel.Ad1;
            blogPost.Ad2 = viewModel.Ad2;
            blogPost.Ad3 = viewModel.Ad3;
            blogPost.IsPostEnabled = viewModel.IsPostEnabled;
            blogPost.BuyMeCoffee = viewModel.BuyMeCoffee;
            blogPost.IsCommentEnabled = viewModel.IsCommentEnabled;
            //blogPost.DatePublished = viewModel.DatePublished;

            blogPost.Tags.Clear();

            foreach (var tagId in viewModel.TagIds)
            {
                var tag = _context.Tags.Where(x => x.Id == tagId).FirstOrDefault();
                blogPost.Tags.Add(tag);
            }

            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        ViewBag.Tags = GetTags();
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteBlogPost(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var blogPost = await _context.BlogPosts.Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        return View(blogPost);
    }


    [HttpPost]
    [ActionName("DeleteBlogPost")]
    public async Task<IActionResult> DeleteBlogPostPost(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var blogPost = await _context.BlogPosts.Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    private List<SelectListItem> GetTags()
    {
        var tags = _context.Tags.Select(tag => new SelectListItem { Value = tag.Id.ToString(), Text = tag.TagDescription }).Cacheable()
            .ToList();
        return tags;
    }
}