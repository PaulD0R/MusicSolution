namespace UserService.Domain.Exceptions;

public class UsernameAlreadyExistsException(string username) 
    : Exception($"User with name '{username}' already exists.")
{
    
}