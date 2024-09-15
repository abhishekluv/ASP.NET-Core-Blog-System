using ASPNETCoreBlog.Models;

namespace ASPNETCoreBlog.ViewModels
{
    public class TagsViewModel
    {
        public IEnumerable<Tag>? Tags { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

}
