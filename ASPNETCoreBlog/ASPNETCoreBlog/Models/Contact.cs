using ASPNETCoreBlog.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? FullName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [EmailAddress]
        [ValidEmailDomain(ErrorMessage = "Email domain is not valid")]
        public string? Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Country { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? City { get; set; }   

        [Required]
        public string? MobileNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateCreated { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 10)]
        public string? Message { get; set; }
    }
}
