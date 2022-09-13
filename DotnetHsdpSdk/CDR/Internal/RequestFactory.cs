using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

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
    private JsonSerializerOptions _options = new JsonSerializerOptions()
        .ForFhir(typeof(Observation).Assembly)
        .ForFhir(typeof(Device).Assembly)
        .ForFhir(typeof(Patient).Assembly);

    public RequestFactory(HsdpCdrConfiguration configuration)
    {
        _tdrEndpoint = configuration.CdrEndpoint;
        _fhirVersion = configuration.FhirVersion;
        _mediaType = configuration.MediaType;
    }

    public IHsdpRequest CreateCdrReadRequest(CdrReadRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Get, new Uri($"{_tdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken,
                modifiedSinceTimestamp: request.ModifiedSinceTimestamp,
                modifiedSinceVersion: request.ModifiedSinceVersion
            ),
            QueryParameters = GetQueryParameters(
                format: request.Format,
                pretty: request.Pretty
            )
        };
    }

    public IHsdpRequest CreateCdrReadVersionRequest(CdrReadVersionRequest request, IIamToken token)
    {
        throw new NotImplementedException("Request is incomplete");
        return new HsdpRequest(HttpMethod.Get, new Uri($"{_tdrEndpoint}/{request.ResourceType}/{request.Id}/_history/{request.VersionId}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken
            ),
            // QueryParameters = request.QueryParameters
        };
    }

    public IHsdpRequest CreateCdrSearchRequest(CdrSearchRequest request, IIamToken token)
    {
        var compartment = request.Compartment;
        var path = compartment != null ? $"{compartment.Type}/{compartment.Id}/{request.ResourceType}" : request.ResourceType;
        throw new NotImplementedException("Request is incomplete");
        if (request.Method == SearchMethod.Get)
        {
            return new HsdpRequest(HttpMethod.Get, new Uri($"{_tdrEndpoint}/{path}"))
            {
                Headers = GetHeaders(
                    accessToken: token.AccessToken
                ),
                // QueryParameters = request.QueryParameters
            };
        }
        else
        {
            return new HsdpRequest(HttpMethod.Post, new Uri($"{_tdrEndpoint}/{path}/_search"))
            {
                Headers = GetHeaders(
                    accessToken: token.AccessToken
                ),
                // QueryParameters = request.QueryParameters
            };
        }

    }

    public IHsdpRequest CreateCdrCreateRequest(CdrCreateRequest request, IIamToken token)
    {
        var jsonContent = JsonContent.Create(
            inputValue: request.Resource,
            inputType: typeof(DomainResource),
            // mediaType: new MediaTypeHeaderValue(_mediaType)
            // {
            //     CharSet = "UTF-8",
            //     Parameters = { new NameValueHeaderValue("fhirVersion", _fhirVersion) }
            // },
            // mediaType: new MediaTypeHeaderValue($"{_mediaType}; fhirVersion={_fhirVersion}"),
            options: _options
        );
        jsonContent.Headers.Remove("Content-Type");
        jsonContent.Headers.TryAddWithoutValidation("Content-Type", $"{_mediaType}; fhirVersion={_fhirVersion}");
        return new HsdpRequest(HttpMethod.Post, new Uri($"{_tdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken,
                shouldValidate: request.ShouldValidate,
                condition:request.Condition,
                preference: request.Preference
            ),
            QueryParameters = GetQueryParameters(
                format: request.Format
            ),
            Content = jsonContent
        };
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

    private List<KeyValuePair<string,string>> GetHeaders(
        string accessToken,
        string? modifiedSinceTimestamp = null, 
        string? modifiedSinceVersion = null,
        bool? shouldValidate = null,
        string? condition = null,
        ReturnPreference? preference = null
    )
    {
        var headers = new List<KeyValuePair<string, string>>
        {
            new("Authorization", $"Bearer {accessToken}"),
            new("api-version", "1"),
            new("Accept", $"{_mediaType}; fhirVersion={_fhirVersion}")
        };
        if (shouldValidate != null)
        {
            headers.Add(new KeyValuePair<string, string>("X-validate-resource", shouldValidate.ToString()!));
        }
        if (condition != null)
        {
            headers.Add(new KeyValuePair<string, string>("If-None-Exists", condition));
        }
        if (preference != null)
        {
            headers.Add(new KeyValuePair<string, string>("Prefer", preference.ToString()!));
        }

        return headers;
    }

    private static List<KeyValuePair<string, string>> GetQueryParameters(
        FormatParameter? format = null,
        bool? pretty = null
    )
    {
        var queryParameters = new List<KeyValuePair<string, string>>();
        if (format != null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("_format",format.ToString()!));
        }
        if (pretty != null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("_pretty", pretty.ToString()!));
        }

        return queryParameters;
    }

}
