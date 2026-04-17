namespace CommentService.Domain.Models;

public class Person
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = [];
}