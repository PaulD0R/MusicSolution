namespace UserService.Application.Interfaces.Factory;

public interface IFactory<out T>
{
    T? Create();
}