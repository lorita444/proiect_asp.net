using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_dubla1.Models
{
    public class Opinion
    {
        public int OpinionId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; } // ex: 1–5

        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public string TargetUserId { get; set; }
        [ForeignKey("TargetUserId")]
        public User TargetUser { get; set; }
    }
}
