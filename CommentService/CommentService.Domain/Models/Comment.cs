using System.ComponentModel.DataAnnotations.Schema;

namespace CommentService.Domain.Models;

public class Comment
{
    public Guid Id { get; set; }
    public string PersonId { get; set; } = null!;
    public Person Person { get; set; } = null!;
    public Guid? MusicId { get; set; }
    public Guid? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public ICollection<Comment> Comments { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    
    [NotMapped] 
    public int ResponseCount { get; set; }
    [NotMapped]
    public readonly ICollection<object> DeleteEvents = [];
}