using System.Collections.Generic;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace DotnetHsdpSdk.CDR;

public class HsdpCdrConfiguration
{
    public HsdpCdrConfiguration(string cdrEndpoint, string fhirVersion, string mediaType)
    {
        Validate.NotNull(cdrEndpoint, nameof(cdrEndpoint));
        Validate.NotNull(fhirVersion, nameof(fhirVersion));
        Validate.NotNull(mediaType, nameof(mediaType));

        CdrEndpoint = cdrEndpoint;
        FhirVersion = fhirVersion;
        MediaType = mediaType;
    }

    public string CdrEndpoint { get; }
    public string FhirVersion { get; }
    public string MediaType { get; }
}

#region Requests

public class CdrReadRequest
{
    public CdrReadRequest(
        string resourceType,
        string id,
        string? modifiedSinceTimestamp = null,
        string? modifiedSinceVersion = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(resourceType));

        ResourceType = resourceType;
        Id = id;
        ModifiedSinceTimestamp = modifiedSinceTimestamp;
        ModifiedSinceVersion = modifiedSinceVersion;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public string? ModifiedSinceTimestamp { get; }
    public string? ModifiedSinceVersion { get; }
}

public class CdrReadVersionRequest
{
    public CdrReadVersionRequest(
        string resourceType,
        string id,
        string versionId
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(resourceType));
        Validate.NotNullOrEmpty(versionId, nameof(versionId));

        ResourceType = resourceType;
        Id = id;
        VersionId = versionId;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public string VersionId { get; }
}

public class CdrSearchRequest
{
    public CdrSearchRequest(
        string resourceType,
        SearchMethod method,
        SearchParameterHandling handlingPreference = SearchParameterHandling.Strict,
        Compartment? compartment = null,
        List<QueryParameter>? queryParameters = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));

        ResourceType = resourceType;
        Method = method;
        HandlingPreference = handlingPreference;
        Compartment = compartment;
        QueryParameters = queryParameters;
    }

    public string ResourceType { get; }
    public SearchMethod Method { get; }
    public SearchParameterHandling HandlingPreference { get; }
    public Compartment? Compartment { get; }
    public List<QueryParameter>? QueryParameters { get; }
}

public class QueryParameter
{
    public QueryParameter(string name, string value)
    {
        Validate.NotNull(name, nameof(name));
        Validate.NotNullOrEmpty(value, nameof(value));

        Name = name;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }
}

public class Compartment
{
    public Compartment(string type, string id)
    {
        Validate.NotNullOrEmpty(type, nameof(type));
        Validate.NotNullOrEmpty(id, nameof(id));

        Type = type;
        Id = id;
    }

    public string Type { get; }
    public string Id { get; }
}

public enum SearchMethod
{
    Get,
    Post
}

public class CdrCreateRequest
{
    public CdrCreateRequest(
        string resourceType,
        DomainResource resource,
        bool? shouldValidate = null,
        string? condition = null,
        ReturnPreference? returnPreference = null)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));

        ResourceType = resourceType;
        Resource = resource;
        ShouldValidate = shouldValidate;
        Condition = condition;
        ReturnPreference = returnPreference;
    }

    public string ResourceType { get; }
    public DomainResource Resource { get; }
    public bool? ShouldValidate { get; }
    public string? Condition { get; }
    public ReturnPreference? ReturnPreference { get; }
}

public enum ReturnPreference
{
    Minimal,
    Representation,
    OperationOutcome
}

public class CdrBatchOrTransactionRequest
{
    public CdrBatchOrTransactionRequest(Bundle bundle, ReturnPreference? returnPreference = null)
    {
        Bundle = bundle;
        ReturnPreference = returnPreference;
    }

    public Bundle Bundle { get; }
    public ReturnPreference? ReturnPreference { get; }
}

public class CdrDeleteByIdRequest
{
    public CdrDeleteByIdRequest(string resourceType, string id)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(id));

        ResourceType = resourceType;
        Id = id;
    }

    public string ResourceType { get; }
    public string Id { get; }
}

public class CdrDeleteByQueryRequest
{
    public CdrDeleteByQueryRequest(string resourceType, List<QueryParameter> queryParameters)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNull(queryParameters, nameof(queryParameters));

        ResourceType = resourceType;
        QueryParameters = queryParameters;
    }

    public string ResourceType { get; }
    public List<QueryParameter> QueryParameters { get; }
}

public class CdrUpdateByIdRequest
{
    public CdrUpdateByIdRequest(
        string resourceType,
        string id,
        DomainResource resource,
        string? forVersion = null,
        bool? shouldValidate = null,
        ReturnPreference? returnPreference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(id));

        ResourceType = resourceType;
        Id = id;
        Resource = resource;
        ForVersion = forVersion;
        ShouldValidate = shouldValidate;
        ReturnPreference = returnPreference;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public DomainResource Resource { get; }
    public string? ForVersion { get; }
    public bool? ShouldValidate { get; }
    public ReturnPreference? ReturnPreference { get; }
}

public class CdrUpdateByQueryRequest
{
    public CdrUpdateByQueryRequest(
        string resourceType,
        List<QueryParameter> queryParameters,
        DomainResource resource,
        string? forVersion = null,
        ReturnPreference? returnPreference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNull(queryParameters, nameof(queryParameters));

        ResourceType = resourceType;
        QueryParameters = queryParameters;
        Resource = resource;
        ForVersion = forVersion;
        ReturnPreference = returnPreference;
    }

    public string ResourceType { get; }
    public List<QueryParameter> QueryParameters { get; }
    public DomainResource Resource { get; }
    public string? ForVersion { get; }
    public ReturnPreference? ReturnPreference { get; }
}

public class CdrPatchByIdRequest
{
    public CdrPatchByIdRequest(
        string resourceType,
        string id,
        List<Operation> operations,
        string? forVersion = null,
        bool? shouldValidate = null,
        ReturnPreference? returnPreference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(id));

        ResourceType = resourceType;
        Id = id;
        Operations = operations;
        ForVersion = forVersion;
        ShouldValidate = shouldValidate;
        ReturnPreference = returnPreference;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public List<Operation> Operations { get; }
    public string? ForVersion { get; }
    public bool? ShouldValidate { get; }
    public ReturnPreference? ReturnPreference { get; }
}

public class CdrPatchByQueryRequest
{
    public CdrPatchByQueryRequest(
        string resourceType,
        List<QueryParameter> queryParameters,
        List<Operation> operations,
        string? forVersion = null,
        ReturnPreference? returnPreference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNull(queryParameters, nameof(queryParameters));

        ResourceType = resourceType;
        QueryParameters = queryParameters;
        Operations = operations;
        ForVersion = forVersion;
        ReturnPreference = returnPreference;
    }

    public string ResourceType { get; }
    public List<QueryParameter> QueryParameters { get; }
    public List<Operation> Operations { get; }
    public string? ForVersion { get; }
    public ReturnPreference? ReturnPreference { get; }
}

#endregion

#region Responses

public class CdrReadResponse
{
    public int Status { get; init; }
    public DomainResource? Resource { get; init; }
}

public class CdrSearchResponse
{
    public int Status { get; init; }
    public Bundle? Bundle { get; init; }
    public OperationOutcome? OperationOutcome { get; init; }
}

public class CdrCreateResponse
{
    public int Status { get; init; }
    public DomainResource? Resource { get; init; }
    public string Location { get; init; } = "";
    public string ETag { get; init; } = "";
    public string LastModified { get; init; } = "";
}

public class CdrBatchOrTransactionResponse
{
    public int Status { get; init; }
    public Bundle? Bundle { get; init; }
    public OperationOutcome? OperationOutcome { get; init; }
}

public class CdrDeleteResponse
{
    public int Status { get; init; }
    public OperationOutcome? OperationOutcome { get; init; }
}

public class CdrUpdateResponse
{
    public int Status { get; init; }
    public DomainResource? Resource { get; init; }
    public string ETag { get; init; } = "";
    public string LastModified { get; init; } = "";
}

public class CdrPatchResponse
{
    public int Status { get; init; }
    public DomainResource? Resource { get; init; }
    public string ETag { get; init; } = "";
    public string LastModified { get; init; } = "";
}

#endregion
