using System;
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
        .ForFhir(typeof(Bundle).Assembly)
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

    public async Task<CdrSearchResponse> Search(CdrSearchRequest searchRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrSearchRequest(searchRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrSearchResponse(response);
    }

    public async Task<CdrCreateResponse> Create(CdrCreateRequest createRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrCreateRequest(createRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrCreateResponse(response);
    }

    public async Task<CdrBatchOrTransactionResponse> BatchOrTransaction(
        CdrBatchOrTransactionRequest batchOrTransactionRequest,
        IIamToken token
    )
    {
        var request = _requestFactory.CreateCdrBatchOrTransactionRequest(batchOrTransactionRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateBatchOrTransactionResponse(response);
    }

    public async Task<CdrDeleteResponse> Delete(CdrDeleteByIdRequest deleteRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrDeleteByIdRequest(deleteRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrDeleteResponse(response);
    }

    public async Task<CdrDeleteResponse> Delete(CdrDeleteByQueryRequest deleteRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrDeleteByQueryRequest(deleteRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrDeleteResponse(response);
    }

    public async Task<CdrUpdateResponse> Update(CdrUpdateByIdRequest updateRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrUpdateByIdRequest(updateRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrUpdateResponse(response);
    }

    public async Task<CdrUpdateResponse> Update(CdrUpdateByQueryRequest updateRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrUpdateByQueryRequest(updateRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrUpdateResponse(response);
    }

    public async Task<CdrPatchResponse> Patch(CdrPatchByIdRequest patchRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrPatchByIdRequest(patchRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrPatchResponse(response);
    }

    public async Task<CdrPatchResponse> Patch(CdrPatchByQueryRequest patchRequest, IIamToken token)
    {
        var request = _requestFactory.CreateCdrPatchByQueryRequest(patchRequest, token);
        var response = await _http.HttpRequest<string>(request);
        return CreateCdrPatchResponse(response);
    }

    private CdrReadResponse CreateCdrReadResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new HsdpRequestException(500, "Response body is missing");
        var resource = JsonSerializer.Deserialize<DomainResource>(body, _options);

        return new CdrReadResponse {
            Status = response.StatusCode,
            Resource = resource
        };
    }

    private CdrSearchResponse CreateCdrSearchResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new Exception("Search failed");
        if (IsSuccessResponse(response.StatusCode))
        {
            var bundle = JsonSerializer.Deserialize<Bundle>(body, _options);
            return new CdrSearchResponse {
                Status = response.StatusCode,
                Bundle = bundle
            };
        }
        else
        {
            var operationOutcome = JsonSerializer.Deserialize<OperationOutcome>(body, _options);
            return new CdrSearchResponse {
                Status = response.StatusCode,
                OperationOutcome = operationOutcome
            };
        }
    }

    private CdrCreateResponse CreateCdrCreateResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new Exception("Resource creation failed");
        var resource = JsonSerializer.Deserialize<DomainResource>(body, _options);

        return new CdrCreateResponse {
            Status = response.StatusCode,
            Resource = resource,
            Location = response.Headers.Find(pair => pair.Key == "Location").Value,
            ETag = response.Headers.Find(pair => pair.Key == "ETag").Value,
            LastModified = response.Headers.Find(pair => pair.Key == "Last-Modified").Value
        };
    }
    
    private CdrBatchOrTransactionResponse CreateBatchOrTransactionResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new Exception("Resource creation failed");
        if (IsSuccessResponse(response.StatusCode))
        {
            var bundle = JsonSerializer.Deserialize<Bundle>(body, _options);

            return new CdrBatchOrTransactionResponse{
                Status = response.StatusCode,
                Bundle = bundle
            };
        }
        else
        {
            var operationOutcome = JsonSerializer.Deserialize<OperationOutcome>(body, _options);

            return new CdrBatchOrTransactionResponse{
                Status = response.StatusCode,
                OperationOutcome = operationOutcome
            };
        }
    }

    private CdrDeleteResponse CreateCdrDeleteResponse(IHsdpResponse<string> response)
    {
        if (IsSuccessResponse(response.StatusCode))
            return new CdrDeleteResponse{
                Status = response.StatusCode,
            };
        {
            var body = response.Body ?? throw new Exception("Resource deletion failed");
            var operationOutcome = JsonSerializer.Deserialize<OperationOutcome>(body, _options);
            return new CdrDeleteResponse{
                Status = response.StatusCode,
                OperationOutcome = operationOutcome
            };
        }
    }

    private CdrUpdateResponse CreateCdrUpdateResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new Exception("Resource creation failed");
        var resource = JsonSerializer.Deserialize<DomainResource>(body, _options);

        return new CdrUpdateResponse {
            Status = response.StatusCode,
            Resource = resource,
            Location = response.Headers.Find(pair => pair.Key == "Location").Value,
            ETag = response.Headers.Find(pair => pair.Key == "ETag").Value,
            LastModified = response.Headers.Find(pair => pair.Key == "Last-Modified").Value
        };
    }

    private CdrPatchResponse CreateCdrPatchResponse(IHsdpResponse<string> response)
    {
        var body = response.Body ?? throw new Exception("Resource creation failed");
        var resource = JsonSerializer.Deserialize<DomainResource>(body, _options);

        return new CdrPatchResponse {
            Status = response.StatusCode,
            Resource = resource,
            Location = response.Headers.Find(pair => pair.Key == "Location").Value,
            ETag = response.Headers.Find(pair => pair.Key == "ETag").Value,
            LastModified = response.Headers.Find(pair => pair.Key == "Last-Modified").Value
        };
    }

    private static bool IsSuccessResponse(int statusCode)
    {
        return statusCode is >= 200 and < 300;
    }

}
