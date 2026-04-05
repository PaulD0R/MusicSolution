namespace UserService.Domain.Exceptions;

public class InternalServerError(string message) : Exception(message);