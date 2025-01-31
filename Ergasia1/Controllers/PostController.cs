using Ergasia1.Data;
using Ergasia1.Entities;
using Ergasia1.Models.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ergasia1.Controllers;
[Authorize]
public class PostController : Controller
{

    #region DI

    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public PostController(
        ApplicationDbContext context,
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new CreatePostVm
        {
            AvailableTags = await _context.Tags.ToListAsync()
            ?? throw new Exception("no tags found")
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePostVm model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableTags = await _context.Tags.ToListAsync();
            return View(model);
        }

        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("user not found");


        var post = new Post
        {
            Title = model.Title,
            Content = model.Content,
            Category = model.Category,
            UserId = userId
        };

        post.Tags = await _context.Tags
            .Where(t => model.SelectedTagIds.Contains(t.Id)).ToListAsync();

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public async Task<IActionResult> List(string searchString, int page = 1)
    {
        int pageSize = 10;

        var postsQuery = _context.Posts
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Tags)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            postsQuery = postsQuery.Where(p => p.Title.Contains(searchString) || p.Content.Contains(searchString));
        }

        var totalPosts = await postsQuery.CountAsync();

        var posts = await postsQuery
            .OrderBy(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new PostListViewModel
        {
            Posts = posts,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize),
            SearchString = searchString,
        };

        return View(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new Exception("Post not found");

        return View(post);
    }

}
