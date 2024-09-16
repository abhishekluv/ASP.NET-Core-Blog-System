using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class HomePageController : BaseController
{
    private readonly BlogSystemContext _context;

    public HomePageController(BlogSystemContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { area = "" });
        var homePage = await _context.HomePage.Cacheable().FirstOrDefaultAsync();
        return View(homePage);
    }

    [HttpPost]
    public async Task<IActionResult> Index(HomePage homePage)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { area = "" });
        if (!ModelState.IsValid) return View(homePage);

        var homePageFromDb = await _context.HomePage.FirstOrDefaultAsync();

        homePageFromDb.Title = homePage.Title;
        homePageFromDb.Content = homePage.Content;
        homePageFromDb.CallToAction = homePage.CallToAction;
        homePageFromDb.BeforeContent = homePage.BeforeContent;
        homePageFromDb.AfterContent = homePage.AfterContent;
        homePageFromDb.FooterContent = homePage.FooterContent;
        homePageFromDb.Image = homePage.Image;
        homePageFromDb.MetaDescription = homePage.MetaDescription;
        homePageFromDb.MetaKeywords = homePage.MetaKeywords;

        _context.HomePage.Update(homePageFromDb);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}