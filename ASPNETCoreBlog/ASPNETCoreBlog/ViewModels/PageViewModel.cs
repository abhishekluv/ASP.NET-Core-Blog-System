using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.ViewModels
{
    public class PageViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
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

        [Required]
        public string? MetaDescription { get; set; }
        public bool IsVisibleToSearchEngine { get; set; }
    }
}
