using CommentService.Application.Models.Comments;
using CommentService.Domain.Models;

namespace CommentService.Application.Mappers;

public static class CommentMapper
{
    extension(Comment comment)
    {
        public CommentDto ToCommentDto(byte[] startPicture) =>
            new()
            {
                Id = comment.Id,
                Name = comment.Person.Name,
                ResponseCount = comment.ResponseCount,
                StartPicture = startPicture,
                CreatedAt = comment.CreatedAt
            };

        public CommentResponseDto ToCommentResponseDto(byte[] startPicture) =>
            new()
            {
                Id = comment.Id,
                Name = comment.Person.Name,
                StartPicture = startPicture,
                CreatedAt = comment.CreatedAt
            };
    }
}