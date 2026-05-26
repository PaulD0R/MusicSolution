namespace RoomService.Application.Interfaces.Factories;

public interface IFactory<out T>
{
    T Create();
}