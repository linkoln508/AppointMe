using AppointMe.Api.Auth;
using System.ComponentModel.DataAnnotations;

namespace AppointMe.Api.Models.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [RegularExpression($"^({Roles.Customer}|{Roles.BusinessOwner})$",
        ErrorMessage = "AccountType must be Customer or BusinessOwner.")]
    public string AccountType { get; set; } = string.Empty;
}