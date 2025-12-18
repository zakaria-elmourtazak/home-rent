using System.ComponentModel.DataAnnotations;
namespace MyMvcAuthProject.Models
{
  public class Conversation
    {
        [Key]
        public int Id { get; set; }

        public string? Title { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
 public int? PropertyId { get; set; }
        public Property? Property { get; set; }
        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}