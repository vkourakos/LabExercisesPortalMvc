using Microsoft.AspNetCore.Identity;

namespace Ergasia1.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    #region Collections
    public List<Contact> Contacts { get; set; } = [];

    public List<Message> SentMessages { get; set; } = [];

    public List<Message> ReceivedMessages { get; set; } = [];
    public List<Post> Posts { get; set; } = [];
    public List<Group> CreatedGroups { get; set; } = [];
    public List<Group> Groups { get; set; } = [];
    #endregion
}
