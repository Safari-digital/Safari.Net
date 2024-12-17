using System.ComponentModel.DataAnnotations;

namespace Digital.Net.Authentication.Controllers.Models;

public class LoginPayload
{
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
}