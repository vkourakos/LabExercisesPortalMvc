using Ergasia1.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ergasia1.Models.Posts;

public class CreatePostVm
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Select at least 1 tag.")]
    [MaxLength(3, ErrorMessage = "You can select up to 3 tags.")]
    public List<int> SelectedTagIds { get; set; } = [];

    public List<Tag> AvailableTags { get; set; } = [];
}
