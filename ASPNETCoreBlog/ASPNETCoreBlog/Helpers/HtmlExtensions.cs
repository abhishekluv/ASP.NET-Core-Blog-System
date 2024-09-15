using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ASPNETCoreBlog.Helpers
{
    public static class HtmlExtensions
    {
        private static readonly HtmlContentBuilder _emptyBuilder = new HtmlContentBuilder();

        public static IHtmlContent BuildBreadcrumbNavigation(this IHtmlHelper helper)
        {
            if (helper.ViewContext.RouteData.Values["controller"].ToString() == "Home" ||
                helper.ViewContext.RouteData.Values["controller"].ToString() == "Account")
            {
                return _emptyBuilder;
            }

            var title = (string)helper.ViewData["Title"];

            string controllerName = helper.ViewContext.RouteData.Values["controller"].ToString();
            string actionName = helper.ViewContext.RouteData.Values["action"].ToString();

            var breadcrumb = new HtmlContentBuilder()
                                .AppendHtml("<nav aria-label=\"breadcrumb\">")
                                .AppendHtml("<ol class=\"breadcrumb\"><li class=\"breadcrumb-item\">")
                                .AppendHtml(helper.ActionLink("Home", "Index", "Home", null, new { title = "Home" }))
                                .AppendHtml("</li>");

            if (helper.ViewContext.RouteData.Values["action"].ToString() != "Index")
            {
                breadcrumb.AppendHtml("<li class=\"breadcrumb-item active\">")
                          .AppendHtml(helper.ActionLink(title.Titleize(), actionName, controllerName, null, new { title = title.Titleize() }))
                          .AppendHtml("</li>");
            }

            if (helper.ViewContext.RouteData.Values["action"].ToString() == "Index")
            {
                breadcrumb.AppendHtml("<li class=\"breadcrumb-item active\">")
                          .AppendHtml(helper.ActionLink(title.Titleize(), actionName, controllerName, null, new { title = title.Titleize() }))
                          .AppendHtml("</li>");
            }

            return breadcrumb.AppendHtml("</ol>").AppendHtml("</nav>");
        }

        public static string Titleize(this string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text).ToSentenceCase();
        }

        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }

        public static string AbsoluteUrl(this IUrlHelper helper)
        {
            return $"{helper.ActionContext.HttpContext.Request.Scheme}://{helper.ActionContext.HttpContext.Request.Host}{helper.ActionContext.HttpContext.Request.PathBase}{helper.ActionContext.HttpContext.Request.Path}";
        }

        public static string AbsoluteRouteUrl(this IUrlHelper urlHelper, string routeName, object routeValues = null)
        {
            string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            return urlHelper.RouteUrl(routeName, routeValues, scheme);
        }
    }
}