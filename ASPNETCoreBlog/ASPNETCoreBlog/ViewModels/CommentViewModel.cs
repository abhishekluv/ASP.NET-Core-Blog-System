using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 2)]
        public string? Email { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 2)]
        public string? Comment { get; set; }
    }
}
