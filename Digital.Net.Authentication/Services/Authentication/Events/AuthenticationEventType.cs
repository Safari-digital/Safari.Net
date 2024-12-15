using System.ComponentModel.DataAnnotations;

namespace Digital.Net.Authentication.Services.Authentication.Events;

public enum AuthenticationEventType
{
    [Display(Name = "login - Success")]
    LoginSuccess,

    [Display(Name = "login - Failure")]
    LoginFailure,

    [Display(Name = "login - Too many requests")]
    LoginTooManyRequests,

    [Display(Name = "logout")]
    Logout,

    [Display(Name = "logout - all devices")]
    LogoutAll
}