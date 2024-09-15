using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Infrastructure;
using ASPNETCoreBlog.Models;
using EFCoreSecondLevelCacheInterceptor;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreBlog.Controllers;

public class ContactController : Controller
{
    private readonly BlogSystemContext _context;

    public ContactController(BlogSystemContext context)
    {
        _context = context;
    }

    [Route("contact-me", Name = "ContactMe")]
    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.ContactPage = GetContactPage();
        return View();
    }

    [Route("contact-me", Name = "ContactMe")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Contact contact)
    {
        if (ModelState.IsValid)
        {
            var sanitizer = new HtmlSanitizer();

            contact.FullName = sanitizer.Sanitize(contact.FullName);
            contact.Message = sanitizer.Sanitize(contact.Message);
            contact.Email = sanitizer.Sanitize(contact.Email);
            contact.Country = sanitizer.Sanitize(contact.Country);
            contact.City = sanitizer.Sanitize(contact.City);
            contact.DateCreated = DateISTTimeZone.GetDateTimeWithCustomTimeZone();
            contact.MobileNumber = sanitizer.Sanitize(contact.MobileNumber.ToString());

            if (!string.IsNullOrWhiteSpace(contact.FullName) && !string.IsNullOrWhiteSpace(contact.Message) &&
                !string.IsNullOrWhiteSpace(contact.Country) && !string.IsNullOrWhiteSpace(contact.City) && !string.IsNullOrWhiteSpace(contact.MobileNumber) && !string.IsNullOrWhiteSpace(contact.Email))
            {
                await _context.Contacts.AddAsync(contact);
                await _context.SaveChangesAsync();

                return RedirectToAction("ThankYou");
            }

            ModelState.AddModelError("", "Something went wrong..");
            return View(contact);
        }

        return View(contact);
    }

    [HttpGet]
    [Route("contact-me/thankyou", Name = "ContactMeThankYou")]
    public IActionResult ThankYou()
    {
        return View();
    }

    private ContactPage GetContactPage()
    {
        var contactPage = _context.ContactPage.Cacheable().FirstOrDefault();
        return contactPage;
    }
}