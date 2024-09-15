﻿using ASPNETCoreBlog.Models;

namespace ASPNETCoreBlog.Infrastructure
{
    public static class ExtensionMethods
    {
        public static IQueryable<Menu> OrderByCustom(this IQueryable<Menu> menuItems)
        {
            return menuItems.OrderBy(x => x.Order).ThenBy(x => x.Title);
        }
    }
}