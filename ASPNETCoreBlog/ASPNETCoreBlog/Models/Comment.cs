using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ASPNETCoreBlog.Infrastructure;

namespace ASPNETCoreBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [ValidEmailDomain(ErrorMessage = "Email domain is not valid")]
        public string? EmailAddress { get; set; }
        public string? CommentMessage { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? PostedOn { get; set; }
        public int BlogPostId { get; set; }

        [ForeignKey("BlogPostId")]
        public BlogPost? BlogPost { get; set; }
    }
}