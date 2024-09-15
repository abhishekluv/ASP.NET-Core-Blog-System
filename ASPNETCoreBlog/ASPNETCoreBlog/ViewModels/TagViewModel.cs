using ASPNETCoreBlog.Models;

namespace ASPNETCoreBlog.ViewModels
{
    public class TagViewModel
    {
        public Tag? Tag { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

}
