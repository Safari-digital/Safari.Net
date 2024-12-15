using Digital.Net.Authentication.Models;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Services.Authorization;

public interface IAuthorizationApiKeyService<TApiUser> : IAuthorizationService
    where TApiUser : EntityGuid, IApiUser;