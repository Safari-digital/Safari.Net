using Digital.Net.Core.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Digital.Net.Authentication.Extensions;

public static class AuthorizationFilterResult
{
    public static Result SetUnauthorisedResult(this AuthorizationFilterContext context)
    {
        var result = new Result();
        result.AddError(new UnauthorizedAccessException());
        context.Result = new JsonResult(result) { StatusCode = 401 };
        return result;
    }
}