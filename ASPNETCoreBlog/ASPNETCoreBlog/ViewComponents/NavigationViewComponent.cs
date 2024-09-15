using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Infrastructure;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCoreBlog.ViewComponents
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent : ViewComponent
    {
        private readonly BlogSystemContext _context;

        public NavigationViewComponent(BlogSystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menu = await _context.Menus.AsNoTracking().Include(x => x.Parent).Include(x => x.SubMenus).Where(x => !x.IsDisabled).AsSingleQuery().OrderByCustom().Cacheable().ToListAsync();

            var viewModel = new MenuViewModel
            {
                MenuItems = menu
            };

            return View(viewModel);
        }
    }
}
