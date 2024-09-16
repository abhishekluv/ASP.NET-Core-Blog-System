using System.Linq.Dynamic.Core;
using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class ContactsController : BaseController
{
    private readonly BlogSystemContext _context;

    public ContactsController(BlogSystemContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        return View();
    }

    [HttpPost]
    [OutputCache(Duration = 10)]
    public async Task<JsonResult> GetContactsAsJson()
    {
        // server-side parameter
        var start = Convert.ToInt32(Request.Form["start"]);
        var length = Convert.ToInt32(Request.Form["length"]);
        string search = Request.Form["search[value]"];
        string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
        string sortDir = Request.Form["order[0][dir]"];

        if (!isAdmin) return Json(null);
        //_context.ChangeTracker.LazyLoadingEnabled = false;
        var contacts = _context.Contacts.OrderByDescending(x => x.Id).AsNoTracking().AsQueryable();

        var totalCount = contacts.Count();

        if (!string.IsNullOrEmpty(search))
            contacts = contacts.Where(x => x.Email.ToLower().Contains(search.ToLower()) ||
                                           x.FullName.ToLower().Contains(search.ToLower()) ||
                                           x.Country.ToLower().Contains(search.ToLower()) ||
                                           x.City.ToLower().Contains(search.ToLower()));

        contacts = contacts.OrderBy(sortColumnName + " " + sortDir);

        contacts = contacts.Skip(start).Take(length);

        var myContacts = await contacts.Select(x => new
        {
            x.Id,
            x.FullName,
            x.Email,
            x.MobileNumber,
            Location = $"{x.Country}({x.City})"
        }).Cacheable().ToListAsync();

        var options = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return Json(new
        {
            data = myContacts,
            draw = Convert.ToInt32(Request.Form["draw"]),
            recordsTotal = totalCount,
            recordsFiltered = totalCount
        }, options);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });

        var contact = await _context.Contacts.FindAsync(id);
        return View(contact);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });

        var contact = await _context.Contacts.FindAsync(id);

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}