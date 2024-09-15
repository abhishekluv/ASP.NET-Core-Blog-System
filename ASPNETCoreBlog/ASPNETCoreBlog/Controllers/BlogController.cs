using ASPNETCoreBlog.Data;
using ASPNETCoreBlog.Infrastructure;
using ASPNETCoreBlog.Models;
using ASPNETCoreBlog.ViewModels;
using EFCoreSecondLevelCacheInterceptor;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CWACoreCMS.Controllers;

public class BlogController : Controller
{
    private readonly BlogSystemContext _context;

    public BlogController(BlogSystemContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Route("comment/{slug}", Name = "Comment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(string slug, [Bind(include: "Comment")] BlogPostViewModelFront viewModel)
    {
        var htmlSanitizer = new HtmlSanitizer();
        var sanitizedSlug = htmlSanitizer.Sanitize(slug);

        var blog = await _context.BlogPosts.Include(x => x.Tags).Include(x => x.Comments).Where(x => x.Slug == sanitizedSlug)
            .AsNoTracking().FirstOrDefaultAsync();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Something went wrong";
            return RedirectToAction("Post",
                new
                {
                    date = blog.DatePublished.Value.ToString("dd"),
                    month = blog.DatePublished.Value.ToString("MM"),
                    year = blog.DatePublished.Value.ToString("yyyy"),
                    slug = blog.Slug
                });
        }

        if (blog.IsCommentEnabled)
        {
            if (blog == null)
            {
                TempData["Error"] = "Something went wrong";
                return RedirectToAction("Post",
                    new
                    {
                        date = blog.DatePublished.Value.ToString("dd"),
                        month = blog.DatePublished.Value.ToString("MM"),
                        year = blog.DatePublished.Value.ToString("yyyy"),
                        slug = blog.Slug
                    });
            }

            var comment = new Comment();

            comment.BlogPostId = blog.Id;
            comment.Name = htmlSanitizer.Sanitize(viewModel.Comment.Name);
            comment.EmailAddress = htmlSanitizer.Sanitize(viewModel.Comment.Email);
            comment.CommentMessage = htmlSanitizer.Sanitize(viewModel.Comment.Comment);
            comment.PostedOn = DateISTTimeZone.GetDateTimeWithCustomTimeZone();
            comment.IsApproved = false;

            if ((comment.Name.Contains("Admin") || comment.Name.Contains("admin")) && !User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You cannot use this Name for a comment";
                return RedirectToAction("Post",
                    new
                    {
                        date = blog.DatePublished.Value.ToString("dd"),
                        month = blog.DatePublished.Value.ToString("MM"),
                        year = blog.DatePublished.Value.ToString("yyyy"),
                        slug = blog.Slug
                    });
            }

            if (!string.IsNullOrWhiteSpace(comment.CommentMessage) && !string.IsNullOrWhiteSpace(comment.Name) &&
                !string.IsNullOrWhiteSpace(comment.EmailAddress))
            {
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Your comment is under moderation. Check back later..";

                return RedirectToAction("Post",
                    new
                    {
                        date = blog.DatePublished.Value.ToString("dd"),
                        month = blog.DatePublished.Value.ToString("MM"),
                        year = blog.DatePublished.Value.ToString("yyyy"),
                        slug = blog.Slug
                    });
            }
        }

        TempData["Error"] = "Something went wrong. Check your comment.";
        return RedirectToAction("Post",
            new
            {
                date = blog.DatePublished.Value.ToString("dd"),
                month = blog.DatePublished.Value.ToString("MM"),
                year = blog.DatePublished.Value.ToString("yyyy"),
                slug = blog.Slug
            });
    }

    [HttpGet]
    [Route("blog/{year:int}/{month:int}/{date:int}/{*slug}", Name = "BlogPosts")]
    public async Task<IActionResult> Post(int year, int month, int date, string slug)
    {
        try
        {
            // Check if the slug is null or empty
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            // Validate if the date components can form a valid date
            if (!DateTime.TryParse($"{year}-{month}-{date}", out DateTime dateFromUrl))
            {
                return NotFound();
            }

            // Retrieve the blog post including related data
            var blog = await _context.BlogPosts
                .Include(x => x.Tags)
                .Include(x => x.Comments.Where(c => c.IsApproved.HasValue && c.IsApproved.Value))
                .AsNoTracking()
                .Cacheable()
                .FirstOrDefaultAsync(x => x.Slug == slug);

            // Check if the blog is null
            if (blog == null)
            {
                return NotFound();
            }

            // Validate the date of the blog post
            if (blog.DatePublished == null || blog.DatePublished.Value.Date != dateFromUrl)
            {
                return NotFound();
            }

            // Map the blog post to the view model
            var viewModel = new BlogPostViewModelFront
            {
                Id = blog.Id,
                Title = blog.Title,
                Slug = blog.Slug,
                CallToAction = blog.CallToAction,
                BeforeContent = blog.BeforeContent,
                Content = blog.Content,
                AfterContent = blog.AfterContent,
                FooterContent = blog.FooterContent,
                Ad3 = blog.Ad3,
                Ad2 = blog.Ad2,
                Ad1 = blog.Ad1,
                DatePublished = blog.DatePublished,
                DateUpdated = blog.DateUpdated,
                Comments = blog.Comments,
                Tags = blog.Tags,
                IsCommentEnabled = blog.IsCommentEnabled,
                IsPostEnabled = blog.IsPostEnabled,
                Image = blog.Image,
                BuyMeCoffee = blog.BuyMeCoffee,
                MetaDescription = blog.MetaDescription,
                MetaKeywords = blog.MetaKeywords
            };

            // Return the view with the view model
            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Log the exception details for debugging purposes (consider using a logging framework)
            // Handle the exception by returning an error view or redirecting to a custom error page
            return NotFound(); // Ensure you have an "Error" view to display error messages
        }
    }

    [HttpGet]
    [Route("blog/archives", Name = "BlogArchives")]
    public async Task<IActionResult> BlogArchive(int page = 1, int pageSize = 10)
    {
        try
        {
            // Check if any unexpected query string parameters are present
            if (Request.Query.Keys.Any(key => key != "page" && key != "pageSize"))
            {
                return NotFound();
            }

            // Count the total number of blog posts
            var totalBlogPosts = await _context.BlogPosts
                .Where(x => x.IsPostEnabled)
                .CountAsync();

            // Calculate the total number of pages
            var totalPages = (int)Math.Ceiling(totalBlogPosts / (double)pageSize);

            // Ensure the current page isn't out of range
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            // Retrieve the blog posts for the current page, including comments that are CWA approved
            var blogPosts = await _context.BlogPosts
                .Include(x => x.Comments.Where(c => c.IsApproved.HasValue && c.IsApproved.Value))
                .Where(x => x.IsPostEnabled)
                .OrderByDescending(x => x.DatePublished)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Cacheable()
                .ToListAsync();

            // Check if there are any blog posts available
            if (blogPosts == null || !blogPosts.Any())
            {
                return NotFound();
            }

            // Create a view model that includes the blog posts and paging information
            var viewModel = new BlogArchiveViewModel
            {
                BlogPosts = blogPosts,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            // Return the view with the view model
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return NotFound(); // Ensure you have an "Error" view to display error messages
        }
    }

    [HttpGet]
    [Route("tags", Name = "Tags")]
    [ResponseCache(CacheProfileName = "BlogsCache")]
    public async Task<IActionResult> Tags(int page = 1, int pageSize = 10)
    {
        // Check if there are any unexpected query string parameters
        if (Request.Query.Keys.Any(key => key != "page" && key != "pageSize"))
        {
            return NotFound();
        }

        try
        {
            // Count the total number of tags
            var totalTags = await _context.Tags.CountAsync();

            // Calculate the total number of pages
            var totalPages = (int)Math.Ceiling(totalTags / (double)pageSize);

            // Ensure the current page isn't out of range
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            // Retrieve tags for the current page, including only posts that are enabled and visible to search engines
            var tags = await _context.Tags
                .Include(t => t.Posts.Where(p => p.IsPostEnabled && p.IsVisibleToSearchEngine))
                .OrderBy(t => t.TagDescription)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Cacheable()
                .ToListAsync();

            if (tags == null || !tags.Any())
            {
                return NotFound();
            }

            // Create a view model to hold the tags and paging information
            var viewModel = new TagsViewModel
            {
                Tags = tags,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [Route("tag/{tagName}", Name = "Tag")]
    [ResponseCache(CacheProfileName = "BlogsCache")]
    public async Task<IActionResult> Tag(string tagName, int page = 1, int pageSize = 10)
    {
        // Check if tagName is null or empty
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return NotFound();
        }

        // Ensure only 'page' and 'pageSize' query strings are present
        var allowedKeys = new HashSet<string> { "page", "pageSize" };
        if (Request.Query.Keys.Any(key => !allowedKeys.Contains(key)))
        {
            return NotFound();
        }

        // Sanitize the input tagName to prevent XSS attacks
        var sanitizer = new HtmlSanitizer();
        var sanitizedTagName = sanitizer.Sanitize(tagName);

        // Check if the sanitized tag name exists in the database
        if (!await _context.Tags.AnyAsync(x => x.Name == sanitizedTagName))
        {
            return NotFound();
        }

        // Retrieve the tag with its associated posts
        var tag = await _context.Tags
            .Include(x => x.Posts.Where(p => p.IsPostEnabled && p.IsVisibleToSearchEngine)
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize))
            .ThenInclude(p => p.Comments.Where(c => c.IsApproved.HasValue && c.IsApproved.Value))
            .AsNoTracking()
            .Cacheable()
            .FirstOrDefaultAsync(x => x.Name == sanitizedTagName);

        // If no tag is found after all conditions, return 404
        if (tag == null)
        {
            return NotFound();
        }

        // Count the total number of posts for the tag
        var totalPosts = await _context.BlogPosts
            .Where(p => p.IsPostEnabled && p.IsVisibleToSearchEngine && p.Tags.Any(t => t.Name == sanitizedTagName))
            .CountAsync();

        // Calculate the total number of pages
        var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

        // Create the ViewModel
        var viewModel = new TagViewModel
        {
            Tag = tag,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize
        };

        // Pass the tag data to the view
        return View(viewModel);
    }
}