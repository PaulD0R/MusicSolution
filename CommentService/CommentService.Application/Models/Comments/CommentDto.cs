namespace CommentService.Application.Models.Comments;

public record CommentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public byte[] StartPicture { get; set; } = null!;
    public int ResponseCount { get; set; }
    public DateTime CreatedAt { get; set; }
}