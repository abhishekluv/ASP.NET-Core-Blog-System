using ASPNETCoreBlog.Models;
using ASPNETCoreBlog.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ASPNETCoreBlog.Data
{
    public class BlogSystemContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public BlogSystemContext(DbContextOptions<BlogSystemContext> options) : base(options)
        {
            
        }

        public DbSet<Page> Pages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<HomePage> HomePage { get; set; }
        public DbSet<ContactPage> ContactPage { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //m to m relationship
            // Configure the many-to-many relationship between BlogPost and Tag
            builder.Entity<BlogPost>()
                .HasMany(bp => bp.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogPostTag",  // Name of the join table
                    j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    j => j.HasOne<BlogPost>().WithMany().HasForeignKey("BlogPostId"));
        }
    }
}
