using Ergasia1.Data;
using Ergasia1.Entities;
using Ergasia1.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Ergasia1.Controllers;
[Authorize]
public class MessageController : Controller
{
    #region DI

    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IHubContext<MessageHub> _hubContext;

    public MessageController(
        ApplicationDbContext context,
        UserManager<User> userManager,
        IHubContext<MessageHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> Chat(string userId = null!, int? groupId = null)
    {
        var currentUser = await _userManager.GetUserAsync(User)
            ?? throw new Exception("user not found");

        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == userId) ||
                        (m.SenderId == userId && m.ReceiverId == currentUser.Id) ||
                        (groupId != null && m.GroupId == groupId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        ViewBag.UserId = userId;
        ViewBag.GroupId = groupId;

        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string receiverId, int? groupId, string content)
    {
        var sender = await _userManager.GetUserAsync(User)
            ?? throw new Exception("User not found");

        var message = new Entities.Message
        {
            SenderId = sender.Id,
            ReceiverId = receiverId,
            GroupId = groupId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var senderUsername = sender.UserName;

        if (groupId != null)
        {
            await _hubContext.Clients.Group($"group_{groupId}").SendAsync("ReceiveMessage", senderUsername, content);
            await _hubContext.Clients.User(sender.Id).SendAsync("ReceiveMessage", senderUsername, content);
        }
        else
        {
            await _hubContext.Clients.User(receiverId).SendAsync("ReceiveMessage", senderUsername, content);
            await _hubContext.Clients.User(sender.Id).SendAsync("ReceiveMessage", senderUsername, content);
        }

        return Ok();
    }


}
