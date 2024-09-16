using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class ContactPageController : BaseController
{
    private readonly BlogSystemContext _context;

    public ContactPageController(BlogSystemContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { area = "" });
        var contactPage = await _context.ContactPage.Cacheable().FirstOrDefaultAsync();
        return View(contactPage);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ContactPage contactPage)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { area = "" });
        if (!ModelState.IsValid) return View(contactPage);

        var contactPageFromDb = await _context.ContactPage.FirstOrDefaultAsync();

        contactPageFromDb.Title = contactPage.Title;
        contactPageFromDb.Content = contactPage.Content;
        contactPageFromDb.CallToAction = contactPage.CallToAction;
        contactPageFromDb.BeforeContent = contactPage.BeforeContent;
        contactPageFromDb.AfterContent = contactPage.AfterContent;
        contactPageFromDb.FooterContent = contactPage.FooterContent;
        contactPageFromDb.Image = contactPage.Image;
        contactPageFromDb.MetaDescription = contactPage.MetaDescription;
        contactPageFromDb.MetaKeywords = contactPage.MetaKeywords;

        _context.ContactPage.Update(contactPageFromDb);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}