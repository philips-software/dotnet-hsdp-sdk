using System;

namespace DotnetHsdpSdk.Utils;

public class HsdpRequestException : Exception
{
    public HsdpRequestException(int statusCode)
    {
        StatusCode = statusCode;
    }

    public HsdpRequestException(int statusCode, string? message) : base(message)
    {
        StatusCode = statusCode;
    }

    public HsdpRequestException(int statusCode, string message, Exception? ex) : base(message, ex)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
