// ...existing code...
using System.Collections.Generic;

namespace MyMvcAuthProject.Models
{
    public class MessagesPageViewModel
    {
        public IEnumerable<Conversation>? Conversations { get; set; }
        public Conversation? CurrentConversation { get; set; }
        public int? CurrentConversationId => CurrentConversation?.Id;
    }
}