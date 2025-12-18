using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MyMvcAuthProject.Hubs
{
    public class ChatHub : Hub
    {
        public Task JoinConversation(string conversationId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, $"conv-{conversationId}");

        public Task LeaveConversation(string conversationId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv-{conversationId}");
    }
}