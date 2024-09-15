using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models.Identity;
using ASPNETCoreBlog.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.Controllers;

public class AccountController : Controller
{
    private readonly BlogSystemContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        BlogSystemContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel viewModel, string? returnUrl)
    {
        var sanitizer = new HtmlSanitizer();

        ViewData["ReturnUrl"] = sanitizer.Sanitize(returnUrl);

        var sanitizedUserName = sanitizer.Sanitize(viewModel.UserName);
        var sanitizedPassword = sanitizer.Sanitize(viewModel.Password);

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(sanitizedUserName, sanitizedPassword, false, true);

            if (result.Succeeded)
            {
                return (string)ViewData["ReturnUrl"] != null
                    ?
                    //return RedirectToAction("Index", "Dashboard", new { Area = "Admin"});
                    LocalRedirect((string)ViewData["ReturnUrl"])
                    : RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid Login Attempt");
            return View(viewModel);
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var isRegister = await _context.SiteSettings.Select(x => x.IsRegister).FirstOrDefaultAsync();

        if (isRegister) return View();

        return RedirectToAction("AccessDenied");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        var isRegister = await _context.SiteSettings.Select(x => x.IsRegister).FirstOrDefaultAsync();

        if (isRegister)
        {
            if (ModelState.IsValid)
            {
                //check if user with an email already exists in the database.
                var userExists = await _userManager.FindByEmailAsync(viewModel.Email);

                if (userExists != null)
                {
                    ModelState.AddModelError("", "Email already exists");
                    return View(viewModel);
                }

                if (userExists == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = viewModel.UserName,
                        Email = viewModel.Email,
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName
                    };

                    //creating the user in the database
                    var result = await _userManager.CreateAsync(user, viewModel.ConfirmPassword);

                    //false
                    if (result.Succeeded) return RedirectToAction("Login", "Account");

                    ModelState.AddModelError("", "Could not create new user in the database");
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }

        return RedirectToAction("AccessDenied");
    }
}