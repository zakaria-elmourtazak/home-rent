// ...existing code...
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMvcAuthProject.Models
{
    public class ConversationParticipant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ConversationId { get; set; }
        [ForeignKey(nameof(ConversationId))]
        public Conversation Conversation { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!; // AspNetUsers.Id

        public bool IsOwner { get; set; } = false;
    }
}