using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace DotnetHsdpSdk.Utils;

public interface IHsdpRequest
{
    HttpMethod Method { get; }
    Uri Path { get; }
    List<KeyValuePair<string, string>> Headers { get; set; }
    List<KeyValuePair<string, string>> QueryParameters { get; set; }
    HttpContent? Content { get; set; }
}

internal class HsdpRequest : IHsdpRequest
{
    public HsdpRequest(
        HttpMethod method,
        Uri path
    )
    {
        Method = method;
        Path = path;
    }
    
    public HttpMethod Method { get; }
    public Uri Path { get; }
    public List<KeyValuePair<string, string>> Headers { get; set; } = new();
    public List<KeyValuePair<string, string>> QueryParameters { get; set; } = new();
    public HttpContent? Content { get; set; }
}

public interface IHsdpResponse

{
    public int StatusCode { get; }
    public List<KeyValuePair<string, string>> Headers { get; }
}

public interface IHsdpResponse<out T> where T:class
{
    public int StatusCode { get; }
    public List<KeyValuePair<string, string>> Headers { get; }
    public T Body { get; }
}

internal class HsdpResponse: IHsdpResponse
{
    public HsdpResponse(HttpStatusCode statusCode, List<KeyValuePair<string, string>> headers)
    {
        StatusCode = (int) statusCode;
        Headers = headers;
    }

    public int StatusCode { get; }
    public List<KeyValuePair<string, string>> Headers { get; }
}

internal class HsdpResponse<T>: IHsdpResponse<T> where T:class
{
    public HsdpResponse(HttpStatusCode statusCode, List<KeyValuePair<string, string>> headers, T body)
    {
        StatusCode = (int) statusCode;
        Headers = headers;
        Body = body;
    }

    public int StatusCode { get; }
    public List<KeyValuePair<string, string>> Headers { get; }
    public T Body { get; }
}
