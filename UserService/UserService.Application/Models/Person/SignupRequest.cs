using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Models.Person;

public record SignupRequest
{
    [Required(ErrorMessage = "Некорректное имя")]
    [MinLength(3, ErrorMessage = "Некорректная длинна имени")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Некорректная почта")]
    [EmailAddress(ErrorMessage = "Некорректная почта")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Некорректный пароль")]
    [MinLength(8, ErrorMessage = "Некорректная длинна пароля")]
    [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).{8,}$", ErrorMessage = "Некорректный пароль")]
    public string? Password { get; set; }
}