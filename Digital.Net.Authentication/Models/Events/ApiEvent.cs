using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Models.Events;

public abstract class ApiEvent(string userAgent, string ipAddress) : EntityId, IApiEvent
{
    public IApiEvent SetApiUser(Guid apiUserId, Type apiUserType)
    {
        ApiUserId = apiUserId;
        ApiUserType = apiUserType.Name;
        return this;
    }

    public IApiEvent SetResult(Result result)
    {
        if (result.HasError)
        {
            var trace = JsonSerializer.Serialize(result.Errors);
            ErrorTrace = trace.Length > 4096 ? trace[..4096] : trace;
            HasError = true;
        }

        return this;
    }

    [Column("UserAgent"), Required, MaxLength(1024)]
    public string UserAgent { get; } = userAgent;

    [Column("IpAddress"), Required, MaxLength(45)]
    public string IpAddress { get; } = ipAddress;

    [Column("ApiUserId")]
    public Guid? ApiUserId { get; private set; }

    [Column("ApiUserType"), MaxLength(128)]
    public string? ApiUserType { get; private set; }

    [Column("HasError"), Required]
    public bool HasError { get; private set; }

    [Column("ErrorTrace"), MaxLength(4096)]
    public string? ErrorTrace { get; private set; }
}