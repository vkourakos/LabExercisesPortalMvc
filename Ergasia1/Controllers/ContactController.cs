using Ergasia1.Data;
using Ergasia1.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ergasia1.Controllers;

[Authorize]
public class ContactController : Controller
{
    #region DI

    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ContactController(
        ApplicationDbContext context,
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    #endregion

    public async Task<IActionResult> ViewContacts()
    {
        var currentUserId = _userManager.GetUserId(User)
            ?? throw new Exception("user not found");

        var contacts = await _context.Contacts
            .Where(c => c.UserId == currentUserId)
            .Include(c => c.ContactUser)
            .ToListAsync();

        return View(contacts);
    }

    [HttpGet]
    public IActionResult AddContact()
    {
        return View(new List<User>());
    }

    [HttpPost]
    public async Task<IActionResult> AddContact(string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return View(new List<User>());
        }

        var currentUserId = _userManager.GetUserId(User)
            ?? throw new Exception("user not found");

        var existingContacts = await _context.Contacts
            .Where(c => c.UserId == currentUserId)
            .Select(c => c.ContactId)
            .ToListAsync();

        var users = await _context.Users
            .Where(u => (u.UserName.Contains(searchString) || u.Email.Contains(searchString))
                && u.Id != currentUserId
                && !existingContacts.Contains(u.Id))
            .ToListAsync();

        return View(users);
    }


    [HttpPost]
    public async Task<IActionResult> AddToContacts(string contactUserId)
    {
        var currentUserId = _userManager.GetUserId(User)
            ?? throw new Exception("user not found");

        var user = await _context.Users
            .Include(u => u.Contacts)
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new Exception("user not found");

        var contactUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == contactUserId)
            ?? throw new Exception("user not found");

        var existingContact = await _context.Contacts
        .AnyAsync(c => (c.UserId == currentUserId && c.ContactId == contactUserId) ||
                       (c.UserId == contactUserId && c.ContactId == currentUserId));

        if (existingContact)
        {
            ModelState.AddModelError("", "You are already contacts with this user.");
            return RedirectToAction(nameof(AddContact));
        }

        var contact = new Contact
        {
            UserId = currentUserId,
            ContactId = contactUserId
        };

        user.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ViewContacts));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveContact(string contactUserId)
    {
        var currentUserId = _userManager.GetUserId(User)
            ?? throw new Exception("user not found");

        var contactToRemove = await _context.Contacts
            .FirstOrDefaultAsync(c => c.UserId == currentUserId
                && c.ContactId == contactUserId);

        if (contactToRemove != null)
        {
            _context.Contacts.Remove(contactToRemove);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(ViewContacts));
    }
}
