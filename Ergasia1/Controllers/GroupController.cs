using Ergasia1.Data;
using Ergasia1.Entities;
using Ergasia1.Models.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ergasia1.Controllers;

[Authorize]
public class GroupController : Controller
{
    #region DI

    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public GroupController(
        ApplicationDbContext context,
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    #endregion

    public async Task<IActionResult> MyGroups(int page = 1)
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new Exception("user not found");

        var pageSize = 10;
        var totalGroups = await _context.Groups
            .Include(g => g.Members)
            .Include(g => g.Creator)
            .Where(g => g.Members.Any(u => u.Id == user.Id))
            .CountAsync();

        var groups = await _context.Groups
            .Include(g => g.Members)
            .Include(g => g.Creator)
            .Where(g => g.Members.Any(u => u.Id == user.Id))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = groups.Select(group => new GroupWithMemberStatusVm
        {
            Group = group,
            IsCreator = group.UserId == user.Id,
            IsMember = group.Members.Any(u => u.Id == user.Id)
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalGroups / (double)pageSize);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(viewModel);
    }

    public async Task<IActionResult> AllGroups(int page = 1)
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new Exception("user not found");

        var pageSize = 10;
        var totalGroups = await _context.Groups
            .Include(g => g.Members)
            .Include(g => g.Creator)
            .CountAsync();

        var groups = await _context.Groups
            .Include(g => g.Members)
            .Include(g => g.Creator)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = groups.Select(group => new GroupWithMemberStatusVm
        {
            Group = group,
            IsCreator = group.UserId == user.Id,
            IsMember = group.Members.Any(u => u.Id == user.Id)
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalGroups / (double)pageSize);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult CreateGroup()
    {
        return View(new CreateGroupVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGroup(CreateGroupVm createGroupVm)
    {
        if (!ModelState.IsValid)
        {
            return View(createGroupVm);
        }

        var user = await _userManager.GetUserAsync(User)
                ?? throw new Exception("user not found");

        var group = new Group
        {
            Name = createGroupVm.Name,
            Description = createGroupVm.Description,
            UserId = user.Id,
        };

        _context.Groups.Add(group);
        group.Members.Add(user);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyGroups));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> JoinGroup(int groupId)
    {
        var user = await _userManager.GetUserAsync(User)
            ?? throw new Exception("user not found");

        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new Exception("group not found"); ;

        if (!group.Members.Contains(user))
        {
            group.Members.Add(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(MyGroups));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LeaveGroup(int groupId)
    {
        var user = await _userManager.GetUserAsync(User)
                    ?? throw new Exception("User not found");

        var group = await _context.Groups
                    .Include(g => g.Members)
                    .FirstOrDefaultAsync(g => g.Id == groupId)
                    ?? throw new Exception("Group not found");

        group.Members.Remove(user);

        if (group.Members.Count == 0)
        {
            _context.Groups.Remove(group);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyGroups));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteGroup(int groupId)
    {
        var user = await _userManager.GetUserAsync(User)
                    ?? throw new Exception("User not found");

        var group = await _context.Groups
                    .Include(g => g.Members)
                    .FirstOrDefaultAsync(g => g.Id == groupId)
                    ?? throw new Exception("Group not found");

        if (group.UserId != user.Id)
        {
            return Unauthorized();
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyGroups));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int groupId)
    {
        var group = await _context.Groups
            .Include(g => g.Creator)
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new Exception("Group not found");

        return View(group);
    }
}
