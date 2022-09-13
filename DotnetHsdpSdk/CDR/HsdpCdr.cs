using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using DotnetHsdpSdk.CDR.Internal;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace DotnetHsdpSdk.CDR;

public class HsdpCdr: IHsdpCdr
{
    private readonly IRequestFactory _requestFactory;
    private readonly IHttpRequester _http;
    private JsonSerializerOptions _options = new JsonSerializerOptions()
        .ForFhir(typeof(Observation).Assembly)
        .ForFhir(typeof(Device).Assembly)
        .ForFhir(typeof(Patient).Assembly);

    public HsdpCdr(HsdpCdrConfiguration configuration)
        : this(new HttpRequester(), new RequestFactory(configuration))
    {
    }

    internal HsdpCdr(IHttpRequester http, IRequestFactory requestFactory)
    {
        _http = http;
        _requestFactory = requestFactory;
    }

    public async Task<CdrReadResponse> Read(CdrReadRequest readRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrReadRequest(readRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrReadResponse(response);
    }

    public async Task<CdrReadResponse> Read(CdrReadVersionRequest readVersionRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrReadVersionRequest(readVersionRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrReadResponse(response);
    }

    public Task<CdrSearchResponse> Search(CdrSearchRequest searchRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }

    public async Task<CdrCreateResponse> Create(CdrCreateRequest createRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrCreateRequest(createRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrCreateResponse(response);
    }

    public Task<CdrBatchOrTransactionResponse> CreateBatchOrTransaction(CdrCreateBatchOrTransactionRequest createBatchOrTransactionRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }

    public Task<CdrDeleteResponse> Delete(CdrDeleteByIdRequest deleteRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }

    public Task<CdrDeleteResponse> Delete(CdrDeleteByQueryRequest deleteRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }

    public Task<CdrUpdateResponse> Update(CdrUpdateByIdRequest updateRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }

    public Task<CdrUpdateResponse> Update(CdrUpdateByQueryRequest updateRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }

    public Task<CdrPatchResponse> Patch(CdrPatchByIdRequest patchRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }

    public Task<CdrPatchResponse> Patch(CdrPatchByQueryRequest patchRequest, IIamToken token)
    {
        throw new System.NotImplementedException();
    }
    
    private CdrReadResponse CreateCdrReadResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new Exception("Reading resource failed");
        var resource = JsonSerializer.Deserialize<DomainResource>(body, _options);

        return new CdrReadResponse {
            Status = (int)HttpStatusCode.OK, // TODO: capture the status code in the IHsdpResponse
            Resource = resource
        };
    }

    private CdrCreateResponse CreateCdrCreateResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new Exception("Resource creation failed");
        var resource = JsonSerializer.Deserialize<DomainResource>(body, _options);

        return new CdrCreateResponse {
            Status = (int)HttpStatusCode.Created, // TODO: capture the status code in the IHsdpResponse
            Resource = resource,
            Location = response.Headers.Find(pair => pair.Key == "Location").Value,
            ETag = response.Headers.Find(pair => pair.Key == "ETag").Value,
            LastModified = response.Headers.Find(pair => pair.Key == "Last-Modified").Value
        };
    }
}
