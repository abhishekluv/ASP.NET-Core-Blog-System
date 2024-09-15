namespace ASPNETCoreBlog.Models
{
    public class Page
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? Content { get; set; }
        public string? CallToAction { get; set; }
        public string? BeforeContent { get; set; }
        public string? AfterContent { get; set; }
        public string? FooterContent { get; set; }
        public string? Image { get; set; }

        //SEO Meta Tags
        public string? MetaKeywords { get; set; }

        public string? MetaDescription { get; set; }
        public bool IsVisibleToSearchEngine { get; set; }
        public DateTime? DatePublished { get; set; }
    }
}
