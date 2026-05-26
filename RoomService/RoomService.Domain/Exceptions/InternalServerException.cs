namespace RoomService.Domain.Exceptions;

public class InternalServerException(string message) : Exception(message);