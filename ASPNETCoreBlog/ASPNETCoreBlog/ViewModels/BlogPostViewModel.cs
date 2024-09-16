using ASPNETCoreBlog.Models;
using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.ViewModels
{
    public class BlogPostViewModel
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }
        public string? Slug { get; set; }

        [Required]
        public string? Content { get; set; }
        public string? CallToAction { get; set; }
        public string? BeforeContent { get; set; }
        public string? AfterContent { get; set; }
        public string? FooterContent { get; set; }

        public DateTime? DatePublished { get; set; }

        [Required]
        public string? Image { get; set; }

        public bool IsFeatured { get; set; }

        public string? MetaKeywords { get; set; }


        [Required]
        public string? MetaDescription { get; set; }

        public bool IsVisibleToSearchEngine { get; set; }

        [Display(Name = "Tags")]
        public int[]? TagIds { get; set; }

        public ICollection<Tag>? Tags { get; set; }

        public bool IsCommentEnabled { get; set; }
        public ICollection<Comment>? Comments { get; set; }

        public string? Ad1 { get; set; }
        public string? Ad2 { get; set; }
        public string? Ad3 { get; set; }

        public bool IsPostEnabled { get; set; }

        public string? BuyMeCoffee { get; set; }

        public CommentViewModel? Comment { get; set; }
    }
}
