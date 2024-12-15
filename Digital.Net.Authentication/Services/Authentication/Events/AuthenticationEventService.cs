using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Services.Options;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;

namespace Digital.Net.Authentication.Services.Authentication.Events;

public class AuthenticationEventService<TApiUser, TEvent>(
    IHttpContextService httpContextService,
    IJwtOptionService jwtOptionService,
    IRepository<TEvent> repository
) : IAuthenticationEventService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
    where TEvent : AuthenticationEvent, new()
{
    public async Task RegisterEventAsync(
        AuthenticationEventType eventType,
        Result result,
        Guid? userId,
        string? payload
    )
    {
        var authEvent = AuthenticationEvent.Create<TEvent>(
            eventType,
            httpContextService.UserAgent,
            httpContextService.IpAddress,
            payload
        );
        authEvent.SetResult(result);

        if (userId.HasValue)
            authEvent.SetApiUser(userId.Value, typeof(TApiUser));

        await repository.CreateAsync(authEvent);
        await repository.SaveAsync();
    }

    public bool HasTooManyAttempts(string payload)
    {
        var threshold = DateTime.UtcNow.Subtract(jwtOptionService.GetLoginAttemptThreshold());
        var count = repository.Count(e =>
            e.CreatedAt > threshold
            && e.EventType == AuthenticationEventType.LoginFailure
            && e.Payload == payload
            && e.IpAddress == httpContextService.IpAddress
        );
        return count >= jwtOptionService.MaxLoginAttempts;
    }
}