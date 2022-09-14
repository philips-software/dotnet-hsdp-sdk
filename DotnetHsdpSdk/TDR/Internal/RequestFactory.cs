using System;
using System.Collections.Generic;
using System.Net.Http;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR.Internal;

public interface IRequestFactory
{
    IHsdpRequest CreateSearchDataItemByUrlRequest(TdrSearchDataByUrlRequest byUrlRequest, IIamToken token, string? requestId = null);
    IHsdpRequest CreateSearchDataItemRequest(TdrSearchDataByQueryRequest byQueryRequest, IIamToken token, string? requestId = null);
}

internal class RequestFactory: IRequestFactory
{
    private const string DataItemPath = "/store/tdr/DataItem";

    private readonly Uri _tdrEndpoint;

    public RequestFactory(HsdpTdrConfiguration configuration)
    {
        _tdrEndpoint = configuration.TdrEndpoint;
    }

    public IHsdpRequest CreateSearchDataItemByUrlRequest(
        TdrSearchDataByUrlRequest byUrlRequest,
        IIamToken token,
        string? requestId
    )
    {
        return new HsdpRequest(HttpMethod.Get, new Uri(byUrlRequest.FullUrl))
        {
            Headers = GetHeaders(token.AccessToken, null, requestId)
        };
    }
    
    public IHsdpRequest CreateSearchDataItemRequest(
        TdrSearchDataByQueryRequest byQueryRequest,
        IIamToken token,
        string? requestId
    )
    {
        return new HsdpRequest(HttpMethod.Get, new Uri(_tdrEndpoint, DataItemPath))
        {
            Headers = GetHeaders(token.AccessToken, null, requestId),
            QueryParameters = byQueryRequest.QueryParameters
        };
    }
    
    private static List<KeyValuePair<string,string>> GetHeaders(
        string accessToken,
        string? contentType,
        string? requestId
    )
    {
        var headers = new List<KeyValuePair<string, string>>
        {
            new("Authorization", $"Bearer {accessToken}"),
            new("api-version", "5")
        };
        if (contentType != null)
        {
            headers.Add(new KeyValuePair<string, string>("Content-Type", contentType));
        }
        if (requestId != null)
        {
            headers.Add(new KeyValuePair<string, string>("HSDP-Request-ID", requestId));
        }
        return headers;
    }

}