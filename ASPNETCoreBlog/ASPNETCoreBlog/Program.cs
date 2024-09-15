using ASPNETCoreBlog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorOptions(options =>
{
    // Clear existing file extensions and add only .cshtml
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    options.AreaPageViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/Admin/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Admin/Views/Shared/{0}.cshtml");
});

builder.Services.ConfigureEFSecondLevelCache(builder.Configuration);
builder.Services.ConfigureEFCore(builder.Configuration);
builder.Services.ConfigureIdentity(builder.Configuration);

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
