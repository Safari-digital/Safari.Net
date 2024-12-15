using Digital.Net.Authentication.Models;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Services.Authentication.ApiUsers;

public interface IApiUserService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
{
    public Guid? GetAuthenticatedUserId();

    public TApiUser? GetAuthenticatedUser();

    public Task<TApiUser?> GetAuthenticatedUserAsync();
}