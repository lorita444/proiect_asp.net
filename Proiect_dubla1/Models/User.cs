using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

namespace Proiect_dubla1.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Description { get; set; }
        public string? ProfileImagePath { get; set; }

        public string? RelationshipStatus { get; set; }

        public string? City { get; set; }
        public string? Country { get; set; }
        public int? Age { get; set; }
        public bool? IsPrivate { get; set; }


        // navigații
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ICollection<Like> Likes { get; set; }

        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }

        public ICollection<Follow> Followers { get; set; }
        public ICollection<Follow> Following { get; set; }

        public ICollection<Opinion> WrittenOpinions { get; set; }
        public ICollection<Opinion> ReceivedOpinions { get; set; }
    }
}
