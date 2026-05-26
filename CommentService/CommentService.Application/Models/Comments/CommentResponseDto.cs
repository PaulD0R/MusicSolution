namespace CommentService.Application.Models.Comments;

public record CommentResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public byte[] StartPicture { get; set; } = null!;
    public DateTime CreatedAt { get; set; } 
}