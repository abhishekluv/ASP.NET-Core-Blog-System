using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.ViewModels
{
    public class RegisterViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
