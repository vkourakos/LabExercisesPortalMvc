namespace Ergasia1.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    #region Colelctions
    public List<Post> Posts { get; set; } = [];
    #endregion
}

