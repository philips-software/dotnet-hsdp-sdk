using System;

namespace DotnetHsdpSdk.Utils;

public class HsdpRequestException : Exception
{
    public HsdpRequestException()
    {
    }
    
    public HsdpRequestException(string? message): base(message)
    {
    }
    
    public HsdpRequestException(string message, Exception? ex): base(message, ex)
    {
    }
}