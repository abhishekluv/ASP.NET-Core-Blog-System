using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class SiteSettingsController : BaseController
{
    private readonly BlogSystemContext _context;

    public SiteSettingsController(BlogSystemContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var siteSettings = await _context.SiteSettings.Cacheable().FirstOrDefaultAsync();
        return View(siteSettings);
    }

    [HttpPost]
    public async Task<IActionResult> Index(SiteSettings siteSettings)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        if (!ModelState.IsValid) return View(siteSettings);
        _context.SiteSettings.Update(siteSettings);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}