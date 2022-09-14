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
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace DotnetHsdpSdk.CDR.Internal;

public interface IRequestFactory
{
    IHsdpRequest CreateCdrReadRequest(CdrReadRequest request, IIamToken token);
    IHsdpRequest CreateCdrReadVersionRequest(CdrReadVersionRequest request, IIamToken token);
    IHsdpRequest CreateCdrSearchRequest(CdrSearchRequest request, IIamToken token);
    IHsdpRequest CreateCdrCreateRequest(CdrCreateRequest request, IIamToken token);
    IHsdpRequest CreateCdrBatchOrTransactionRequest(CdrBatchOrTransactionRequest request, IIamToken token);
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
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        .ForFhir(typeof(Observation).Assembly)
        .ForFhir(typeof(Device).Assembly)
        .ForFhir(typeof(Patient).Assembly);

    public RequestFactory(HsdpCdrConfiguration configuration)
    {
        _tdrEndpoint = configuration.CdrEndpoint;
        _fhirVersion = configuration.FhirVersion;
        _mediaType = configuration.MediaType;
        _options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
                format: request.Format
            )
        };
    }

    public IHsdpRequest CreateCdrReadVersionRequest(CdrReadVersionRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Get, new Uri($"{_tdrEndpoint}/{request.ResourceType}/{request.Id}/_history/{request.VersionId}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken
            ),
            QueryParameters = GetQueryParameters(
                format: request.Format
            )
        };
    }

    public IHsdpRequest CreateCdrSearchRequest(CdrSearchRequest request, IIamToken token)
    {
        var compartment = request.Compartment;
        var path = compartment != null ? $"{compartment.Type}/{compartment.Id}/{request.ResourceType}" : request.ResourceType;
        if (request.Method == SearchMethod.Get)
        {
            return new HsdpRequest(HttpMethod.Get, new Uri($"{_tdrEndpoint}/{path}"))
            {
                Headers = GetHeaders(
                    accessToken: token.AccessToken
                ),
                QueryParameters = GetQueryParameters(
                    requestQueryParameters: request.QueryParameters,
                    format: request.Format
                )
            };
        }
        else
        {
            return new HsdpRequest(HttpMethod.Post, new Uri($"{_tdrEndpoint}/{path}/_search"))
            {
                Headers = GetHeaders(
                    accessToken: token.AccessToken
                ),
                Content = new FormUrlEncodedContent(GetQueryParameters(
                    requestQueryParameters: request.QueryParameters,
                    format: request.Format
                ))
            };
        }

    }

    public IHsdpRequest CreateCdrCreateRequest(CdrCreateRequest request, IIamToken token)
    {
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
            Content = ToJsonContent(request.Resource)
        };
    }

    public IHsdpRequest CreateCdrBatchOrTransactionRequest(CdrBatchOrTransactionRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Post, _tdrEndpoint)
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken,
                preference: request.Preference
            ),
            QueryParameters = GetQueryParameters(
                format: request.Format
            ),
            Content = ToJsonContent(request.Bundle)
        };
    }

    public IHsdpRequest CreateCdrDeleteByIdRequest(CdrDeleteByIdRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Delete, new Uri($"{_tdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken
            )
        };
    }

    public IHsdpRequest CreateCdrDeleteByQueryRequest(CdrDeleteByQueryRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Delete, new Uri($"{_tdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken
            ),
            QueryParameters = GetQueryParameters(
                requestQueryParameters: request.QueryParameters
            )
        };
    }

    public IHsdpRequest CreateCdrUpdateByIdRequest(CdrUpdateByIdRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Put, new Uri($"{_tdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken,
                modifiedSinceVersion: request.ForVersion,
                shouldValidate: request.ShouldValidate,
                preference: request.Preference
            ),
            QueryParameters = GetQueryParameters(
                format: request.Format
            ),
            Content = ToJsonContent(request.Resource)
        };
    }

    public IHsdpRequest CreateCdrUpdateByQueryRequest(CdrUpdateByQueryRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Put, new Uri($"{_tdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken,
                modifiedSinceVersion: request.ForVersion,
                preference: request.Preference
            ),
            QueryParameters = GetQueryParameters(
                requestQueryParameters: request.QueryParameters,
                format: request.Format
            ),
            Content = ToJsonContent(request.Resource)
        };
    }

    public IHsdpRequest CreateCdrPatchByIdRequest(CdrPatchByIdRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Patch, new Uri($"{_tdrEndpoint}/{request.ResourceType}/{request.Id}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken,
                matchesVersion: request.ForVersion,
                shouldValidate: request.ShouldValidate,
                preference: request.Preference
            ),
            QueryParameters = GetQueryParameters(
                format: request.Format
            ),
            Content = ToJsonContent(request.Operations, "application/json-patch+json")
        };
    }

    public IHsdpRequest CreateCdrPatchByQueryRequest(CdrPatchByQueryRequest request, IIamToken token)
    {
        return new HsdpRequest(HttpMethod.Patch, new Uri($"{_tdrEndpoint}/{request.ResourceType}"))
        {
            Headers = GetHeaders(
                accessToken: token.AccessToken,
                matchesVersion: request.ForVersion,
                preference: request.Preference
            ),
            QueryParameters = GetQueryParameters(
                requestQueryParameters: request.QueryParameters,
                format: request.Format
            ),
            Content = ToJsonContent(request.Operations, "application/json-patch+json")
        };
    }

    private List<KeyValuePair<string,string>> GetHeaders(
        string accessToken,
        string? modifiedSinceTimestamp = null, 
        string? modifiedSinceVersion = null,
        string? matchesVersion = null,
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
        if (modifiedSinceTimestamp != null)
        {
            headers.Add(new KeyValuePair<string, string>("If-Modified-Since", modifiedSinceTimestamp));
        }
        if (modifiedSinceVersion != null)
        {
            headers.Add(new KeyValuePair<string, string>("If-None-Match", modifiedSinceVersion));
        }
        if (matchesVersion != null)
        {
            headers.Add(new KeyValuePair<string, string>("If-Match", matchesVersion));
        }
        if (shouldValidate != null)
        {
            headers.Add(new KeyValuePair<string, string>("X-validate-resource", shouldValidate.ToString()!.ToLower()));
        }
        if (condition != null)
        {
            headers.Add(new KeyValuePair<string, string>("If-None-Exists", condition));
        }
        if (preference != null)
        {
            headers.Add(new KeyValuePair<string, string>("Prefer", $"return={preference.ToString()!.ToLower()}"));
        }

        return headers;
    }

    private static List<KeyValuePair<string, string>> GetQueryParameters(
        IEnumerable<QueryParameter>? requestQueryParameters = null,
        FormatParameter? format = null
    )
    {
        var queryParameters = ToKeyValuePairs(requestQueryParameters) ?? new List<KeyValuePair<string, string>>();
        if (format != null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("_format",format.ToString()!));
        }

        return queryParameters;
    }

    private static List<KeyValuePair<string, string>>? ToKeyValuePairs(IEnumerable<QueryParameter>? requestQueryParameters)
    {
        return requestQueryParameters
            ?.Select(qp => new KeyValuePair<string, string>(qp.Name, qp.Value))
            .ToList();
    }

    private JsonContent ToJsonContent(Bundle bundle)
    {
        var jsonContent = JsonContent.Create(
            inputValue: bundle,
            inputType: typeof(Bundle),
            options: _options
        );
        jsonContent.Headers.Remove("Content-Type");
        jsonContent.Headers.TryAddWithoutValidation("Content-Type", $"{_mediaType}; fhirVersion={_fhirVersion}");
        return jsonContent;
    }

    private JsonContent ToJsonContent(DomainResource resource)
    {
        var jsonContent = JsonContent.Create(
            inputValue: resource,
            inputType: typeof(DomainResource),
            options: _options
        );
        jsonContent.Headers.Remove("Content-Type");
        jsonContent.Headers.TryAddWithoutValidation("Content-Type", $"{_mediaType}; fhirVersion={_fhirVersion}");
        return jsonContent;
    }

    private JsonContent ToJsonContent(List<Operation> operations, string mediaType)
    {
        var jsonContent = JsonContent.Create(
            inputValue: operations,
            inputType: typeof(List<Operation>),
            options: _options
        );
        jsonContent.Headers.Remove("Content-Type");
        jsonContent.Headers.TryAddWithoutValidation("Content-Type", mediaType);
        return jsonContent;
    }

}
