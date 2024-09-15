using ASPNETCoreBlog.Models;

namespace ASPNETCoreBlog.ViewModels
{
    public class BlogArchiveViewModel
    {
        public IEnumerable<BlogPost>? BlogPosts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

}
