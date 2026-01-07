using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_dubla1.Models
{
    public class Follow
    {

        public string FollowerId { get; set; }
        [ForeignKey("FollowerId")]
        public User Follower { get; set; }

        public string FollowedId { get; set; }
        [ForeignKey("FollowedId")]
        public User Followed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
