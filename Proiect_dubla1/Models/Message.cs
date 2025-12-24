using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_dubla1.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }

        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }
    }
}
