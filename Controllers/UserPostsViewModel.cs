using Grubhub.Models;
using Grubhub.Controllers;

namespace Grubhub.Controllers
{
    public class UserPostsViewModel
    {
        public User User { get; set; }
        public List<GrabberPost> Posts { get; set; }
    }
}