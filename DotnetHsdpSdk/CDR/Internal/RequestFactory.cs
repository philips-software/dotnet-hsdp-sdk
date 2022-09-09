using System;
using System.Collections.Generic;
using System.Net.Http;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.CDR.Internal;

public interface IRequestFactory
{
    IHsdpRequest CreateCdrReadRequest(CdrReadRequest request, IIamToken token);
    IHsdpRequest CreateCdrReadVersionRequest(CdrReadVersionRequest request, IIamToken token);
    IHsdpRequest CreateCdrSearchRequest(CdrSearchRequest request, IIamToken token);
    IHsdpRequest CreateCdrCreateRequest(CdrCreateRequest request, IIamToken token);
    IHsdpRequest CreateCdrCreateBatchOrTransactionRequest(CdrCreateBatchOrTransactionRequest request, IIamToken token);
    IHsdpRequest CreateCdrDeleteByIdRequest(CdrDeleteByIdRequest request, IIamToken token);
    IHsdpRequest CreateCdrDeleteByQueryRequest(CdrDeleteByQueryRequest request, IIamToken token);
    IHsdpRequest CreateCdrUpdateByIdRequest(CdrUpdateByIdRequest request, IIamToken token);
    IHsdpRequest CreateCdrUpdateByQueryRequest(CdrUpdateByQueryRequest request, IIamToken token);
    IHsdpRequest CreateCdrPatchByIdRequest(CdrPatchByIdRequest request, IIamToken token);
    IHsdpRequest CreateCdrPatchByQueryRequest(CdrPatchByQueryRequest request, IIamToken token);
}

internal class RequestFactory: IRequestFactory
{
    private const string DataItemPath = "/store/tdr/DataItem";

    private readonly Uri _tdrEndpoint;
    private readonly string _fhirVersion;
    private readonly string _mediaType;

    public RequestFactory(HsdpCdrConfiguration configuration)
    {
        _tdrEndpoint = configuration.CdrEndpoint;
        _fhirVersion = configuration.FhirVersion;
        _mediaType = configuration.MediaType;
    }

    public IHsdpRequest CreateCdrReadRequest(CdrReadRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Get, _tdrEndpoint)
        {
            Headers = GetHeaders(token.AccessToken, null),
            // QueryParameters = request.QueryParameters

        };
    }

    public IHsdpRequest CreateCdrReadVersionRequest(CdrReadVersionRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrSearchRequest(CdrSearchRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrCreateRequest(CdrCreateRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrCreateBatchOrTransactionRequest(CdrCreateBatchOrTransactionRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrDeleteByIdRequest(CdrDeleteByIdRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrDeleteByQueryRequest(CdrDeleteByQueryRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrUpdateByIdRequest(CdrUpdateByIdRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrUpdateByQueryRequest(CdrUpdateByQueryRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrPatchByIdRequest(CdrPatchByIdRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    public IHsdpRequest CreateCdrPatchByQueryRequest(CdrPatchByQueryRequest request, IIamToken token)
    {
        throw new NotImplementedException();
    }

    private static List<KeyValuePair<string,string>> GetHeaders(
        string accessToken,
        string? contentType
    )
    {
        var headers = new List<KeyValuePair<string, string>>
        {
            new("Authorization", $"Bearer {accessToken}"),
            new("api-version", "1")
        };
        if (contentType != null)
        {
            headers.Add(new KeyValuePair<string, string>("Content-Type", contentType));
        }
        return headers;
    }

}