using Digital.Net.Core.Messages;

namespace Digital.Net.Authentication.Models.Events;

public interface IApiEvent
{
    public IApiEvent SetApiUser(Guid apiUserId, Type apiUserType);
    public IApiEvent SetResult(Result result);
    public string UserAgent { get; }
    public string IpAddress { get; }
    public Guid? ApiUserId { get; }
    public string? ApiUserType { get; }
    public bool HasError { get; }
    public string? ErrorTrace { get; }
}