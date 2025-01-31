using Ergasia1.Entities;

namespace Ergasia1.Models.Groups;

public class GroupWithMemberStatusVm
{
    public Group Group { get; set; }
    public bool IsMember { get; set; }
    public bool IsCreator { get; set; }
}
