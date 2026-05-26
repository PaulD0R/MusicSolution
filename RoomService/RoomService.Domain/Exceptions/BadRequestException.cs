namespace RoomService.Domain.Exceptions;

public class BadRequestException(string message) : Exception(message);