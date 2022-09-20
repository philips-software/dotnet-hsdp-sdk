using System.Text.Json;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace DotnetHsdpSdk.CDR.Internal;

public interface ICdrResponseFactory
{
    CdrReadResponse CreateCdrReadResponse(IHsdpResponse<string> response);
    CdrSearchResponse CreateCdrSearchResponse(IHsdpResponse<string> response);
    CdrCreateResponse CreateCdrCreateResponse(IHsdpResponse<string> response);
    CdrBatchOrTransactionResponse CreateCdrBatchOrTransactionResponse(IHsdpResponse<string> response);
    CdrDeleteResponse CreateCdrDeleteResponse(IHsdpResponse<string> response);
    CdrUpdateResponse CreateCdrUpdateResponse(IHsdpResponse<string> response);
    CdrPatchResponse CreateCdrPatchResponse(IHsdpResponse<string> response);
}

internal class CdrResponseFactory : ICdrResponseFactory
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        .ForFhir(typeof(Bundle).Assembly)
        .ForFhir(typeof(Observation).Assembly)
        .ForFhir(typeof(Device).Assembly)
        .ForFhir(typeof(Patient).Assembly);

    public CdrReadResponse CreateCdrReadResponse(IHsdpResponse<string> response)
    {
        return new CdrReadResponse
        {
            Status = response.StatusCode,
            Resource = Deserialize<DomainResource>(response.Body)
        };
    }

    public CdrSearchResponse CreateCdrSearchResponse(IHsdpResponse<string> response)
    {
        if (response.StatusCode.IsSuccess())
            return new CdrSearchResponse
            {
                Status = response.StatusCode,
                Bundle = Deserialize<Bundle>(response.Body)
            };
        return new CdrSearchResponse
        {
            Status = response.StatusCode,
            OperationOutcome = Deserialize<OperationOutcome>(response.Body)
        };
    }

    public CdrCreateResponse CreateCdrCreateResponse(IHsdpResponse<string> response)
    {
        return new CdrCreateResponse
        {
            Status = response.StatusCode,
            Resource = Deserialize<DomainResource>(response.Body),
            Location = response.Headers.Find(pair => pair.Key == "Location").Value,
            ETag = response.Headers.Find(pair => pair.Key == "ETag").Value,
            LastModified = response.Headers.Find(pair => pair.Key == "Last-Modified").Value
        };
    }

    public CdrBatchOrTransactionResponse CreateCdrBatchOrTransactionResponse(IHsdpResponse<string> response)
    {
        if (response.StatusCode.IsSuccess())
            return new CdrBatchOrTransactionResponse
            {
                Status = response.StatusCode,
                Bundle = Deserialize<Bundle>(response.Body)
            };
        return new CdrBatchOrTransactionResponse
        {
            Status = response.StatusCode,
            OperationOutcome = Deserialize<OperationOutcome>(response.Body)
        };
    }

    public CdrDeleteResponse CreateCdrDeleteResponse(IHsdpResponse<string> response)
    {
        if (response.StatusCode.IsSuccess())
            return new CdrDeleteResponse
            {
                Status = response.StatusCode
            };
        {
            return new CdrDeleteResponse
            {
                Status = response.StatusCode,
                OperationOutcome = Deserialize<OperationOutcome>(response.Body)
            };
        }
    }

    public CdrUpdateResponse CreateCdrUpdateResponse(IHsdpResponse<string> response)
    {
        return new CdrUpdateResponse
        {
            Status = response.StatusCode,
            Resource = Deserialize<DomainResource>(response.Body),
            Location = response.Headers.Find(pair => pair.Key == "Location").Value,
            ETag = response.Headers.Find(pair => pair.Key == "ETag").Value,
            LastModified = response.Headers.Find(pair => pair.Key == "Last-Modified").Value
        };
    }

    public CdrPatchResponse CreateCdrPatchResponse(IHsdpResponse<string> response)
    {
        return new CdrPatchResponse
        {
            Status = response.StatusCode,
            Resource = Deserialize<DomainResource>(response.Body),
            Location = response.Headers.Find(pair => pair.Key == "Location").Value,
            ETag = response.Headers.Find(pair => pair.Key == "ETag").Value,
            LastModified = response.Headers.Find(pair => pair.Key == "Last-Modified").Value
        };
    }

    private T Deserialize<T>(string? body)
    {
        if (body == null) throw new HsdpRequestException(InternalApiError.StatusCode, "Response body is missing");
        try
        {
            var resource = JsonSerializer.Deserialize<T>(body, _options);
            if (resource == null)
                throw new HsdpRequestException(InternalApiError.StatusCode, "Deserialized body is null");

            return resource;
        }
        catch (DeserializationFailedException e)
        {
            throw new HsdpRequestException(InternalApiError.StatusCode, "Unexpected data in response", e);
        }
        catch (JsonException e)
        {
            throw new HsdpRequestException(InternalApiError.StatusCode, "Invalid JSON", e);
        }
    }
}
