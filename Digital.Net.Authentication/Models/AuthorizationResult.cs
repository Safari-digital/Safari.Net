using Digital.Net.Core.Messages;

namespace Digital.Net.Authentication.Models;

public class AuthorizationResult : Result
{
    public bool IsAuthorized { get; private set; }

    public void Authorize() => IsAuthorized = true;

    public new AuthorizationResult Merge(Result result)
    {
        base.Merge(result);
        if (result is AuthorizationResult authResult)
            IsAuthorized = authResult.IsAuthorized;
        return this;
    }
}