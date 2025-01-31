using Ergasia1.Entities;

namespace Ergasia1.Models.Posts;

public class PostListViewModel
{
    public List<Post> Posts { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string SearchString { get; set; }
}
