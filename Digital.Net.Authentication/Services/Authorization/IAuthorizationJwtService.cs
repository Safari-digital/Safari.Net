using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Models.Authorizations;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Services.Authorization;

public interface IAuthorizationJwtService<TApiUser> : IAuthorizationService
    where TApiUser : EntityGuid, IApiUser
{
    public AuthorizationResult AuthorizeApiUserRefresh(string? token);
}