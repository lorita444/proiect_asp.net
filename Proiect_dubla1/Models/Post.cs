using System.Xml.Linq;

namespace Proiect_dubla1.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }


        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
