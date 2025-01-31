using Ergasia1.Data;
using Ergasia1.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Ergasia1.Hubs;

public class MessageHub : Hub
{
    #region DI

    private readonly ApplicationDbContext _context;

    public MessageHub(ApplicationDbContext context)
    {
        _context = context;
    }

    #endregion

    public async Task SendMessage(string senderId, string receiverId, int? groupId, string content)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            GroupId = groupId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(senderId);
        string senderUsername = sender?.UserName ?? "Unknown User";

        if (groupId != null)
        {
            await Clients.Group($"group_{groupId}").SendAsync("ReceiveMessage", senderUsername, content);
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderUsername, content);
        }
        else
        {
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderUsername, content);
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderUsername, content);
        }
    }

    public async Task JoinGroup(int groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"group_{groupId}");
    }

    public async Task LeaveGroup(int groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group_{groupId}");
    }
}

