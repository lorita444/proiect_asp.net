using System.ComponentModel.DataAnnotations;

namespace Proiect_dubla1.Models
{
    public class Post
    {
        public int PostId { get; set; }

        [Required]
        public PostType Type { get; set; }

        public string? Content { get; set; }      // pentru Thought
        public string? Caption { get; set; }      // pentru ImagePost
        public string? ImagePath { get; set; }    // pentru ImagePost

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; } = "";

        public User? User { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
