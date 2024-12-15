using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Authentication.Models.Events;
using Digital.Net.Core.Extensions.EnumUtilities;

namespace Digital.Net.Authentication.Services.Authentication.Events;

public abstract class AuthenticationEvent(
    AuthenticationEventType eventType,
    string userAgent,
    string ipAddress,
    string? payload
) : ApiEvent(userAgent, ipAddress)
{
    [Column("Action"), Required]
    public AuthenticationEventType EventType { get; private set; } = eventType;

    [Column("ActionName"), Required, MaxLength(128)]
    public string ActionName { get; private set; } = eventType.GetDisplayName();

    [Column("Payload")]
    public string? Payload { get; private set; } = payload;

    public static TEvent Create<TEvent>(
        AuthenticationEventType eventType,
        string userAgent,
        string ipAddress,
        string? payload = null
    ) where TEvent : AuthenticationEvent, new()
    {
        var eventInstance = new TEvent();
        eventInstance.Initialize(eventType, userAgent, ipAddress, payload);
        return eventInstance;
    }

    protected abstract void Initialize(
        AuthenticationEventType eventType,
        string userAgent,
        string ipAddress,
        string? payload
    );
}