// ...existing code...
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Hubs;
using MyMvcAuthProject.Models;

namespace MyMvcAuthProject.Controllers;
public class MessagesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHubContext<ChatHub> _hub;

    public MessagesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHubContext<ChatHub> hub)
    {
        _db = db;
        _userManager = userManager;
        _hub = hub;
    }

   [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartConversationFromProperty(int propertyId, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return BadRequest("Message required");

        // var user = await _userManager.GetUserAsync(User);
        // if (user == null) return Unauthorized();
var user = new ApplicationUser { Id = "user2-id", UserName = "user2", Email = "user2@example.com"};
        var prop = await _db.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
        if (prop == null) return NotFound();
        var ownerId = prop.UserId;
        if (ownerId == user.Id) return BadRequest("Cannot message yourself");

        // Try to find an existing one-to-one conversation between these two users for THIS property
        var conv = await _db.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c =>
                c.PropertyId == propertyId &&                                  // match property
                c.Participants.Any(p => p.UserId == user.Id) &&
                c.Participants.Any(p => p.UserId == ownerId) &&
                c.Participants.Count == 2
            );

        if (conv == null)
        {
            conv = new Conversation
            {
                Title = $"Inquiry about {prop.Title ?? $"Property {prop.Id}"}",
                PropertyId = propertyId,
                LastUpdated = DateTime.UtcNow
            };
            _db.Conversations.Add(conv);
            await _db.SaveChangesAsync();

            _db.ConversationParticipants.Add(new ConversationParticipant { ConversationId = conv.Id, UserId = user.Id });
            _db.ConversationParticipants.Add(new ConversationParticipant { ConversationId = conv.Id, UserId = ownerId, IsOwner = true });
            await _db.SaveChangesAsync();
        }

        var msg = new Message
        {
            ConversationId = conv.Id,
            SenderId = user.Id,
            SenderName = user.UserName ?? user.Email,
            Content = message,
            SentAt = DateTime.UtcNow
        };
        _db.Messages.Add(msg);
        conv.LastUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // broadcast to conversation group
        var dto = new { id = msg.Id, conversationId = conv.Id, senderId = msg.SenderId, senderName = msg.SenderName, content = msg.Content, sentAt = msg.SentAt.ToString("o") };
        await _hub.Clients.Group($"conv-{conv.Id}").SendAsync("ReceiveMessage", dto);

        // redirect to Messages view opening this conversation
        return RedirectToAction(nameof(Messages), new { currentConversationId = conv.Id });
    }
    

      [HttpGet]
    public async Task<IActionResult> Messages(int? currentConversationId)
    {
        // var user = await _userManager.GetUserAsync(User);
        // if (user == null) return Challenge(); // or RedirectToAction("Login","Account");
var user = new ApplicationUser { Id = "user2-id", UserName = "user2", Email = "user2@example.com"};

        // load conversations where the user participates
        var conversations = await _db.Conversations
            .Where(c => c.Participants.Any(p => p.UserId == user.Id))
            .Include(c => c.Participants)
            .Include(c => c.Messages)
            .OrderByDescending(c => c.LastUpdated)
            .ToListAsync();

        MyMvcAuthProject.Models.MessagesPageViewModel vm = new MyMvcAuthProject.Models.MessagesPageViewModel
        {
            Conversations = conversations
        };

        if (currentConversationId.HasValue)
        {
            var conv = conversations.FirstOrDefault(c => c.Id == currentConversationId.Value)
                       ?? await _db.Conversations
                                .Include(c => c.Participants)
                                .Include(c => c.Messages)
                                .FirstOrDefaultAsync(c => c.Id == currentConversationId.Value);

            if (conv != null && conv.Participants.Any(p => p.UserId == user.Id))
            {
                vm.CurrentConversation = conv;
            }
        }
        // the view file is under Views/Home/messages.cshtml — render it explicitly
        return View("~/Views/Home/messages.cshtml", vm);
    }

    

    // list conversations for current user
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var convs = await _db.Conversations
            .Where(c => c.Participants.Any(p => p.UserId == user.Id))
            .OrderByDescending(c => c.LastUpdated)
            .Select(c => new
            {
                c.Id,
                c.Title,
                LastUpdated = c.LastUpdated,
                LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()
            })
            .ToListAsync();

        // map to viewmodel or pass raw and render
        return View(convs);
    }

    // view conversation
    public async Task<IActionResult> Conversation(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var conv = await _db.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages.OrderBy(m => m.SentAt))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conv == null) return NotFound();
        if (!conv.Participants.Any(p => p.UserId == user.Id)) return Forbid();

        return View(conv);
    }

    [HttpGet]
    public async Task<IActionResult> ConversationMessages(int id)
    {
        // try real user, fallback to test user used elsewhere in this controller
        var user = await _userManager.GetUserAsync(User)
                   ?? new ApplicationUser { Id = "user2-id", UserName = "user2", Email = "user2@example.com" };

        var conv = await _db.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conv == null) return NotFound();

        if (!conv.Participants.Any(p => p.UserId == user.Id))
            return Forbid();

        var messages = conv.Messages
            .OrderBy(m => m.SentAt)
            .Select(m => new {
                id = m.Id,
                senderId = m.SenderId,
                senderName = m.SenderName,
                content = m.Content,
                sentAt = m.SentAt.ToString("o")
            })
            .ToList();

        var participants = conv.Participants.Select(p => p.UserId).ToList();

        return Json(new {
            id = conv.Id,
            title = conv.Title,
            propertyId = conv.PropertyId,
            participants,
            messages
        });
    }

    // send message to existing conversation (AJAX or form post)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(int conversationId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return BadRequest("Empty");
Console.WriteLine("SendMessage called");
        // get current user (replace test user with real _userManager.GetUserAsync(User) in production)
        // var user = await _userManager.GetUserAsync(User);
        // if (user == null) return Unauthorized();
var user = new ApplicationUser { Id = "user2-id", UserName = "user2", Email = "user2@example.com"};
        var conv = await _db.Conversations.Include(c => c.Participants).FirstOrDefaultAsync(c => c.Id == conversationId);
        if (conv == null) return NotFound();
        if (!conv.Participants.Any(p => p.UserId == user.Id)) return Forbid();

        var msg = new Message
        {
            ConversationId = conversationId,
            SenderId = user.Id,
            SenderName = user.UserName ?? user.Email,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _db.Messages.Add(msg);
        conv.LastUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var dto = new
        {
            id = msg.Id,
            conversationId = conversationId,
            senderId = msg.SenderId,
            senderName = msg.SenderName,
            content = msg.Content,
            sentAt = msg.SentAt.ToString("o")
        };

        // broadcast to SignalR group
        await _hub.Clients.Group($"conv-{conversationId}").SendAsync("ReceiveMessage", dto);

        // If request is AJAX (fetch/XHR), return JSON DTO so client can update immediately.
        var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                     || Request.Headers["Accept"].ToString().Contains("application/json");
        if (isAjax)
        {
            return Ok(dto);
        }
        // Otherwise redirect back to messages view and open the conversation
        return RedirectToAction(nameof(Messages), new { currentConversationId = conversationId });
    }
}