using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.Models
{
    public class Tag
    {
        public Tag()
        {
            Posts = new List<BlogPost>();
        }
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public string? TagDescription { get; set; }
        public string? TagSummary { get; set; }
        public string? TagImage { get; set; }
        public ICollection<BlogPost>? Posts { get; set; }

        public DateTime? DatePublished { get; set; }
    }
}