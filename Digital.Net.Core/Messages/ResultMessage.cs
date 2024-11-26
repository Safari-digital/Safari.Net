﻿using Digital.Net.Core.Extensions.EnumUtilities;
using Digital.Net.Core.Extensions.ExceptionUtilities;

namespace Digital.Net.Core.Messages;

/// <summary>
///     A class to hold the result of a message. Can be created using either an exception or an enum.
/// </summary>
public class ResultMessage
{
    public ResultMessage(Exception ex, Enum? message = null)
    {
        Code = message?.GetHashCode().ToString() ?? ex.GetFormattedErrorCode();
        Message = message?.GetDisplayName() ?? ex.Message;
        Reference = message?.ToReferenceString() ?? ex.GetReference();
        StackTrace = ex.StackTrace;
    }

    public ResultMessage(Enum message)
    {
        Code = message.GetHashCode().ToString();
        Reference = message.ToReferenceString();
        Message = message.GetDisplayName();
    }

    public string? Code { get; init; }
    public string? Reference { get; init; }
    public string? Message { get; init; }
    public string? StackTrace { get; init; }
}