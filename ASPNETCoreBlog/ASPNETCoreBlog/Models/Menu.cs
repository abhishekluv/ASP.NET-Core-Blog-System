using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.Models
{
    public class Menu
    {
        public Menu()
        {
            SubMenus = new HashSet<Menu>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        //[Url]
        public string? Url { get; set; }

        public int? Order { get; set; }

        public bool IsDisabled { get; set; }

        [Display(Name = "Parent")]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Menu? Parent { get; set; }

        [InverseProperty("Parent")]
        public ICollection<Menu> SubMenus { get; set; }
    }
}
