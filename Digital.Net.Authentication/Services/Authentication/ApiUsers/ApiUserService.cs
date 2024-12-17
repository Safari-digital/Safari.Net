using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Models.Authorizations;
using Digital.Net.Authentication.Options;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;

namespace Digital.Net.Authentication.Services.Authentication.ApiUsers;

public class ApiUserService<TApiUser>(
    IHttpContextService httpContextService,
    IRepository<TApiUser> repository
) : IApiUserService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
{
    public Guid? GetAuthenticatedUserId() =>
        httpContextService.GetItem<AuthorizationResult>(AuthenticationDefaults.ApiContextAuthorizationKey)?.ApiUserId;

    public TApiUser? GetAuthenticatedUser() => repository.GetById(GetAuthenticatedUserId());

    public Task<TApiUser?> GetAuthenticatedUserAsync() => repository.GetByIdAsync(GetAuthenticatedUserId());
}