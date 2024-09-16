using System.Linq.Dynamic.Core;
using ASPNETCoreBlog.Areas.Admin.Controllers;
using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CWACoreCMS.Areas.Admin.Controllers;

[Area("Admin")]
public class MenusController : BaseController
{
    private readonly BlogSystemContext _context;

    public MenusController(BlogSystemContext context)
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
    public async Task<JsonResult> GetMenusAsJson()
    {
        // Extract and parse parameters efficiently
        if (!int.TryParse(Request.Form["start"], out var start)) start = 0;
        if (!int.TryParse(Request.Form["length"], out var length)) length = 10;
        string search = Request.Form["search[value]"];
        search = !string.IsNullOrEmpty(search) ? search.ToLower() : null;
        string sortColumnName = Request.Form[$"columns[{Request.Form["order[0][column]"]}][name]"];
        string sortDir = Request.Form["order[0][dir]"];
        sortDir = !string.IsNullOrEmpty(sortDir) && sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        // Return null if the user is not an admin
        if (!isAdmin) return Json(null);

        // Prepare the query
        var menusQuery = _context.Menus
            .AsNoTracking()
            .AsSplitQuery()
            .OrderByDescending(x => x.Id)
            .AsQueryable();

        var totalCount = await menusQuery.CountAsync();

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(search))
        {
            menusQuery = menusQuery.Where(x => x.Title.ToLower().Contains(search));
        }

        // Apply sorting dynamically
        menusQuery = menusQuery.OrderBy($"{sortColumnName} {sortDir}");

        // Apply pagination
        var paginatedMenus = await menusQuery
            .Skip(start)
            .Take(length)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.IsDisabled,
                x.Url,
                x.Order,
                Parent = x.Parent.Title ?? "None"
            })
            .Cacheable()
            .ToListAsync();

        // Serialize and return the response
        return Json(new
        {
            data = paginatedMenus,
            draw = int.TryParse(Request.Form["draw"], out var draw) ? draw : 1,
            recordsTotal = totalCount,
            recordsFiltered = totalCount
        }, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        ViewBag.MenusDropDown = await GetMenusDropDownListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Menu menu)
    {
        if (ModelState.IsValid)
        {
            if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        ViewBag.MenusDropDown = await GetMenusDropDownListAsync();
        return View(menu);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var menu = await _context.Menus.SingleOrDefaultAsync(x => x.Id == id);
        ViewBag.MenusDropDown = await GetMenusDropDownListAsync();
        return View(menu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Menu menu)
    {
        if (ModelState.IsValid)
        {
            if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        ViewBag.MenusDropDown = await GetMenusDropDownListAsync();
        return View(menu);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        if (!isAdmin) return RedirectToAction("AccessDenied", "Account", new { Area = "" });
        var menu = await _context.Menus.SingleOrDefaultAsync(x => x.Id == id);
        return View(menu);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int id)
    {
        var menu = await _context.Menus.SingleOrDefaultAsync(x => x.Id == id);

        try
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Cannot Delete Menu as it has a submenu: {ex.InnerException.Message}");
            return View(menu);
        }

        return RedirectToAction("Index");
    }

    private async Task<List<SelectListItem>> GetMenusDropDownListAsync()
    {
        //var menus = await _context.Menus.Where(x => !x.ParentId.HasValue).Cacheable().ToListAsync();
        var menus = await _context.Menus.Cacheable().ToListAsync();

        var dropDown = new List<SelectListItem>();


        // Recursive method to add items to the dropdown list in hierarchical order
        void AddMenuItemsToDropdown(Menu item, IEnumerable<Menu> allMenus, int level)
        {
            string prefix = new string('-', level * 2); // Create a prefix based on the level (2 dashes per level)
            dropDown.Add(new SelectListItem { Text = $"{prefix}{item.Title}", Value = item.Id.ToString() });

            var childItems = allMenus.Where(m => m.ParentId == item.Id).OrderBy(m => m.Order).ToList();

            foreach (var child in childItems)
            {
                AddMenuItemsToDropdown(child, allMenus, level + 1);
            }
        }

        // Start with top-level menus (those without a ParentId)
        foreach (var menu in menus.Where(m => !m.ParentId.HasValue).OrderBy(m => m.Order).ToList())
        {
            AddMenuItemsToDropdown(menu, menus, 0);
        }

        return dropDown;
    }
}