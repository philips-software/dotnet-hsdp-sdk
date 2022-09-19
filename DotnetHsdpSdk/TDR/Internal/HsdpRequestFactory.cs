using System;
using System.Collections.Generic;
using System.Net.Http;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR.Internal;

public interface IHsdpRequestFactory
{
    IHsdpRequest Create(TdrSearchDataByUrlRequest request, IIamToken token, string? requestId = null);
    IHsdpRequest Create(TdrSearchDataByQueryRequest request, IIamToken token, string? requestId = null);
}

internal class HsdpRequestFactory : IHsdpRequestFactory
{
    private const string DataItemPath = "store/tdr/DataItem";

    private readonly string _tdrEndpoint;

    public HsdpRequestFactory(HsdpTdrConfiguration configuration)
    {
        _tdrEndpoint = configuration.TdrEndpoint;
    }

    public IHsdpRequest Create(TdrSearchDataByUrlRequest request, IIamToken token, string? requestId = null)
    {
        return new HsdpRequest(HttpMethod.Get, new Uri(request.FullUrl))
        {
            Headers = GetHeaders(token.AccessToken, null, requestId)
        };
    }

    public IHsdpRequest Create(TdrSearchDataByQueryRequest request, IIamToken token, string? requestId = null)
    {
        return new HsdpRequest(HttpMethod.Get, new Uri($"{_tdrEndpoint}/{DataItemPath}"))
        {
            Headers = GetHeaders(token.AccessToken, null, requestId),
            QueryParameters = request.QueryParameters
        };
    }

    private static List<KeyValuePair<string, string>> GetHeaders(
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
        if (contentType != null) headers.Add(KeyValuePair.Create("Content-Type", contentType));
        if (requestId != null) headers.Add(KeyValuePair.Create("HSDP-Request-ID", requestId));
        return headers;
    }
}
