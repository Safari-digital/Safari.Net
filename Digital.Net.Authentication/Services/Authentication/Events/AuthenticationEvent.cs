using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Authentication.Models.Events;
using Digital.Net.Core.Extensions.EnumUtilities;

namespace Digital.Net.Authentication.Services.Authentication.Events;

public abstract class AuthenticationEvent : ApiEvent
{
    public AuthenticationEvent()
    {
    }

    public AuthenticationEvent(
        AuthenticationEventType eventType,
        string userAgent,
        string ipAddress,
        string? payload = null
    )
    {
        Initialize(eventType, userAgent, ipAddress, payload);
    }

    [Column("Action"), Required]
    public AuthenticationEventType EventType { get; private set; } = AuthenticationEventType.Unknown;

    [Column("ActionName"), Required, MaxLength(128)]
    public string ActionName { get; private set; } = string.Empty;

    [Column("Payload")]
    public string? Payload { get; private set; } = string.Empty;

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

    protected void Initialize(
        AuthenticationEventType eventType,
        string userAgent,
        string ipAddress,
        string? payload
    )
    {
        EventType = eventType;
        ActionName = eventType.GetDisplayName();
        UserAgent = userAgent;
        IpAddress = ipAddress;
        Payload = payload;
    }
}