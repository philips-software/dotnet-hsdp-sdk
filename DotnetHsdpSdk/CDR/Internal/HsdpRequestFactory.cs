using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;

namespace DotnetHsdpSdk.CDR.Internal;

public interface IHsdpRequestFactory
{
    IHsdpRequest Create(CdrReadRequest request, IIamToken token);
    IHsdpRequest Create(CdrReadVersionRequest request, IIamToken token);
    IHsdpRequest Create(CdrSearchRequest request, IIamToken token);
    IHsdpRequest Create(CdrCreateRequest request, IIamToken token);
    IHsdpRequest Create(CdrBatchOrTransactionRequest request, IIamToken token);
    IHsdpRequest Create(CdrDeleteByIdRequest request, IIamToken token);
    IHsdpRequest Create(CdrDeleteByQueryRequest request, IIamToken token);
    IHsdpRequest Create(CdrUpdateByIdRequest request, IIamToken token);
    IHsdpRequest Create(CdrUpdateByQueryRequest request, IIamToken token);
    IHsdpRequest Create(CdrPatchByIdRequest request, IIamToken token);
    IHsdpRequest Create(CdrPatchByQueryRequest request, IIamToken token);
}

internal class HsdpRequestFactory : IHsdpRequestFactory
{
    private readonly string _cdrEndpoint;
    private readonly string _fhirVersion;
    private readonly string _mediaType;

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        .ForFhir(typeof(Observation).Assembly)
        .ForFhir(typeof(Device).Assembly)
        .ForFhir(typeof(Patient).Assembly);

    public HsdpRequestFactory(HsdpCdrConfiguration configuration)
    {
        _cdrEndpoint = configuration.CdrEndpoint;
        _fhirVersion = configuration.FhirVersion;
        _mediaType = configuration.MediaType;
        _options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }

    public IHsdpRequest Create(CdrReadRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Get, new Uri($"{_cdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                token.AccessToken,
                request.ModifiedSinceTimestamp,
                request.ModifiedSinceVersion
            ),
            QueryParameters = GetQueryParameters()
        };
    }

    public IHsdpRequest Create(CdrReadVersionRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Get,
            new Uri($"{_cdrEndpoint}/{request.ResourceType}/{request.Id}/_history/{request.VersionId}"))
        {
            Headers = GetHeaders(
                token.AccessToken
            ),
            QueryParameters = GetQueryParameters()
        };
    }

    public IHsdpRequest Create(CdrSearchRequest request, IIamToken token)
    {
        var compartment = request.Compartment;
        var path = compartment != null
            ? $"{compartment.Type}/{compartment.Id}/{request.ResourceType}"
            : request.ResourceType;
        var headers = GetHeaders(
            accessToken: token.AccessToken,
            searchHandlingPreference: request.HandlingPreference
        );
        if (request.Method == SearchMethod.Get)
            return new HsdpRequest(HttpMethod.Get, new Uri($"{_cdrEndpoint}/{path}"))
            {
                Headers = headers,
                QueryParameters = GetQueryParameters(request.QueryParameters)
            };
        return new HsdpRequest(HttpMethod.Post, new Uri($"{_cdrEndpoint}/{path}/_search"))
        {
            Headers = headers,
            Content = new FormUrlEncodedContent(GetQueryParameters(request.QueryParameters))
        };
    }

    public IHsdpRequest Create(CdrCreateRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Post, new Uri($"{_cdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                token.AccessToken,
                shouldValidate: request.ShouldValidate,
                condition: request.Condition,
                returnPreference: request.ReturnPreference
            ),
            QueryParameters = GetQueryParameters(),
            Content = ToJsonContent(request.Resource, $"{_mediaType}; fhirVersion={_fhirVersion}")
        };
    }

    public IHsdpRequest Create(CdrBatchOrTransactionRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Post, new Uri(_cdrEndpoint))
        {
            Headers = GetHeaders(
                token.AccessToken,
                returnPreference: request.ReturnPreference
            ),
            QueryParameters = GetQueryParameters(),
            Content = ToJsonContent(request.Bundle, $"{_mediaType}; fhirVersion={_fhirVersion}")
        };
    }

    public IHsdpRequest Create(CdrDeleteByIdRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Delete, new Uri($"{_cdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                token.AccessToken
            )
        };
    }

    public IHsdpRequest Create(CdrDeleteByQueryRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Delete, new Uri($"{_cdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                token.AccessToken
            ),
            QueryParameters = GetQueryParameters(request.QueryParameters)
        };
    }

    public IHsdpRequest Create(CdrUpdateByIdRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Put, new Uri($"{_cdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                token.AccessToken,
                modifiedSinceVersion: request.ForVersion,
                shouldValidate: request.ShouldValidate,
                returnPreference: request.ReturnPreference
            ),
            QueryParameters = GetQueryParameters(),
            Content = ToJsonContent(request.Resource, $"{_mediaType}; fhirVersion={_fhirVersion}")
        };
    }

    public IHsdpRequest Create(CdrUpdateByQueryRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Put, new Uri($"{_cdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                token.AccessToken,
                modifiedSinceVersion: request.ForVersion,
                returnPreference: request.ReturnPreference
            ),
            QueryParameters = GetQueryParameters(request.QueryParameters),
            Content = ToJsonContent(request.Resource, $"{_mediaType}; fhirVersion={_fhirVersion}")
        };
    }

    public IHsdpRequest Create(CdrPatchByIdRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Patch, new Uri($"{_cdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                token.AccessToken,
                matchesVersion: request.ForVersion,
                shouldValidate: request.ShouldValidate,
                returnPreference: request.ReturnPreference
            ),
            QueryParameters = GetQueryParameters(),
            Content = ToJsonContent(request.Operations, "application/json-patch+json")
        };
    }

    public IHsdpRequest Create(CdrPatchByQueryRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Patch, new Uri($"{_cdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                token.AccessToken,
                matchesVersion: request.ForVersion,
                returnPreference: request.ReturnPreference
            ),
            QueryParameters = GetQueryParameters(request.QueryParameters),
            Content = ToJsonContent(request.Operations, "application/json-patch+json")
        };
    }

    private List<KeyValuePair<string, string>> GetHeaders(
        string accessToken,
        string? modifiedSinceTimestamp = null,
        string? modifiedSinceVersion = null,
        string? matchesVersion = null,
        bool? shouldValidate = null,
        string? condition = null,
        ReturnPreference? returnPreference = null,
        SearchParameterHandling? searchHandlingPreference = null
    )
    {
        var headers = new List<KeyValuePair<string, string>>
        {
            new("Authorization", $"Bearer {accessToken}"),
            new("api-version", "1"),
            new("Accept", $"{_mediaType}; fhirVersion={_fhirVersion}")
        };
        if (modifiedSinceTimestamp != null)
            headers.Add(KeyValuePair.Create("If-Modified-Since", modifiedSinceTimestamp));
        if (modifiedSinceVersion != null) headers.Add(KeyValuePair.Create("If-None-Match", modifiedSinceVersion));
        if (matchesVersion != null) headers.Add(KeyValuePair.Create("If-Match", matchesVersion));
        if (shouldValidate != null)
            headers.Add(KeyValuePair.Create("X-validate-resource", shouldValidate.ToString()!.ToLower()));
        if (condition != null) headers.Add(KeyValuePair.Create("If-None-Exists", condition));
        if (returnPreference != null)
            headers.Add(KeyValuePair.Create("Prefer", $"return={returnPreference.ToString()!.ToLower()}"));
        if (searchHandlingPreference != null)
            headers.Add(KeyValuePair.Create("Prefer", $"handling={searchHandlingPreference.GetLiteral()}"));

        return headers;
    }

    private static List<KeyValuePair<string, string>> GetQueryParameters(
        IEnumerable<QueryParameter>? requestQueryParameters = null
    )
    {
        return requestQueryParameters?.Select(qp => KeyValuePair.Create(qp.Name, qp.Value)).ToList()
               ?? new List<KeyValuePair<string, string>>();
    }

    private JsonContent ToJsonContent<T>(T content, string mediaType)
    {
        var jsonContent = JsonContent.Create(
            content,
            typeof(T),
            options: _options
        );
        jsonContent.Headers.Remove("Content-Type");
        jsonContent.Headers.TryAddWithoutValidation("Content-Type", mediaType);
        return jsonContent;
    }
}
