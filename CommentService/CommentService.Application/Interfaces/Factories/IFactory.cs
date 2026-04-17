namespace CommentService.Application.Interfaces.Factories;

public interface IFactory<out T>
{
    T Create();
}