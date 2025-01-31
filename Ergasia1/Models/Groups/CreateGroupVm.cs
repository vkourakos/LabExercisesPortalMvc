using System.ComponentModel.DataAnnotations;

namespace Ergasia1.Models.Groups;

public class CreateGroupVm
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(250)]
    public string Description { get; set; }
}
