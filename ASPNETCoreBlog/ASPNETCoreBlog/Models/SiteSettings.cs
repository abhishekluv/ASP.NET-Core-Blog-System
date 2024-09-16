namespace ASPNETCoreBlog.Models
{
    public class SiteSettings
    {
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public bool IsRegister { get; set; }
        public string? LogoURL { get; set; }
        public string? FaviconURL { get; set; }
        public string? SiteAuthor { get; set; }

        public string? BlogAuthorSummary { get; set; }
        public string? SiteFooter { get; set; }
        public string? GoogleSiteVerification { get; set; }
        public string? GoogleAnalytics { get; set; }
        public bool IsCustomCSSOn { get; set; }
        public string? CustomCSS { get; set; }
        public bool IsCustomJSOn { get; set; }
        public string? CustomJavaScript { get; set; }
        public string? DNSPreconnect { get; set; }
        public string? DNSPrefetch { get; set; }
    }
}
