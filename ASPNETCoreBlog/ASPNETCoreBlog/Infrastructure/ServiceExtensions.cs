using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Models.Identity;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigureEFCore(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<BlogSystemContext>((serviceProvider, options) =>
                options.UseSqlServer(config.GetConnectionString("CWACoreCMS"), x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                       .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>())
            );
        }

        public static void ConfigureEFSecondLevelCache(this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();

            services.AddEFSecondLevelCache(options =>
                    options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(double.Parse(config["EFCore2ndLevelCache"]))) // Use memory cache
                           .ConfigureLogging(enable: false)
                           .UseCacheKeyPrefix("EF_"));
        }

        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration config)
        {
            var identitySettings = config.GetSection("IdentitySettings");

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = identitySettings.GetValue<bool>("RequireUniqueEmail");
                options.Password.RequiredLength = identitySettings.GetValue<int>("RequiredLength");
            }).AddEntityFrameworkStores<BlogSystemContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".aspnetcore.blog.auth.cookie";
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                //options.Cookie.Domain = ".dojohaven.com";
            });

            services.Configure<PasswordHasherOptions>(options => options.IterationCount = 250000);

            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        }
    }
}