using Digital.Net.Authentication.Models;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Services.Authentication;

public interface IAuthenticationService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
{
    public Result<string> RefreshTokens();
    public Task<Result<TApiUser>> ValidateCredentials(string login, string password);
    public Task<Result<string>> Login(string login, string password);
    public Task<Result> Logout();
    public Task<Result> LogoutAll();
}