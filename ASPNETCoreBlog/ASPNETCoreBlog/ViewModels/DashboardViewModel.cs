using ASPNETCoreBlog.Models;

namespace ASPNETCoreBlog.ViewModels
{
    public class DashboardViewModel
    {
        public int ContactCount { get; set; }
        public List<Contact>? Top10Contacts { get; set; }
        public int CommentCount { get; set; }
        public int CommentApprovedCount { get; set; }
        public List<Comment>? Top10Comments { get; set; }
    }
}
