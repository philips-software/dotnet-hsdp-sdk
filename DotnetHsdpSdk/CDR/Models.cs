using System;
using System.Collections.Generic;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace DotnetHsdpSdk.CDR;

public class HsdpCdrConfiguration
{
    public HsdpCdrConfiguration(Uri cdrEndpoint, string fhirVersion, string mediaType)
    {
        Validate.NotNull(cdrEndpoint, nameof(cdrEndpoint));
        Validate.NotNull(fhirVersion, nameof(fhirVersion));
        Validate.NotNull(mediaType, nameof(mediaType));

        CdrEndpoint = cdrEndpoint;
        FhirVersion = fhirVersion;
        MediaType = mediaType;
    }

    public Uri CdrEndpoint { get; }
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
        string? modifiedSinceVersion = null,
        FormatParameter? format = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(resourceType));

        ResourceType = resourceType;
        Id = id;
        ModifiedSinceTimestamp = modifiedSinceTimestamp;
        ModifiedSinceVersion = modifiedSinceVersion;
        Format = format;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public string? ModifiedSinceTimestamp { get; }
    public string? ModifiedSinceVersion { get; }
    public FormatParameter? Format { get; }
    public bool? Pretty { get; }
}

public class CdrReadVersionRequest
{
    public CdrReadVersionRequest(
        string resourceType,
        string id,
        string versionId,
        FormatParameter? format = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(resourceType));
        Validate.NotNullOrEmpty(versionId, nameof(versionId));

        ResourceType = resourceType;
        Id = id;
        VersionId = versionId;
        Format = format;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public string VersionId { get; }
    public FormatParameter? Format { get; }
}

public enum FormatParameter
{
    Xml,
    TextXml,
    ApplicationXml,
    ApplicationFhirXml,
    Json,
    ApplicationJson,
    ApplicationFhirJson
}

public class CdrSearchRequest
{
    public CdrSearchRequest(
        string resourceType,
        SearchMethod method,
        Compartment? compartment = null,
        List<QueryParameter>? queryParameters = null,
        FormatParameter? format = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));

        ResourceType = resourceType;
        Method = method;
        Compartment = compartment;
        QueryParameters = queryParameters;
        Format = format;
    }

    public string ResourceType { get; }
    public SearchMethod Method { get; }
    public Compartment? Compartment { get; }
    public List<QueryParameter>? QueryParameters { get; }
    public FormatParameter? Format { get; }
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
    Post,
}

public class CdrCreateRequest
{
    public CdrCreateRequest(
        string resourceType,
        DomainResource resource,
        bool? shouldValidate = null,
        FormatParameter? format = null,
        string? condition = null,
        ReturnPreference? preference = null)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));

        ResourceType = resourceType;
        Resource = resource;
        ShouldValidate = shouldValidate;
        Format = format;
        Condition = condition;
        Preference = preference;
    }

    public string ResourceType { get; }
    public DomainResource Resource { get; }
    public bool? ShouldValidate { get; }
    public FormatParameter? Format { get; }
    public string? Condition { get; }
    public ReturnPreference? Preference { get; }
}

public enum ReturnPreference
{
    Minimal,
    Representation,
    OperationOutcome,
}

public class CdrBatchOrTransactionRequest
{
    public CdrBatchOrTransactionRequest(Bundle bundle, FormatParameter? format = null, ReturnPreference? preference = null)
    {
        Bundle = bundle;
        Format = format;
        Preference = preference;
    }

    public Bundle Bundle { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? Preference { get; }
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
        FormatParameter? format = null,
        ReturnPreference? preference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(id));

        ResourceType = resourceType;
        Id = id;
        Resource = resource;
        ForVersion = forVersion;
        ShouldValidate = shouldValidate;
        Format = format;
        Preference = preference;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public DomainResource Resource { get; }
    public string? ForVersion { get; }
    public bool? ShouldValidate { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? Preference { get; }
}

public class CdrUpdateByQueryRequest
{
    public CdrUpdateByQueryRequest(
        string resourceType,
        List<QueryParameter> queryParameters,
        DomainResource resource,
        string? forVersion = null,
        FormatParameter? format = null,
        ReturnPreference? preference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNull(queryParameters, nameof(queryParameters));

        ResourceType = resourceType;
        QueryParameters = queryParameters;
        Resource = resource;
        ForVersion = forVersion;
        Format = format;
        Preference = preference;
    }

    public string ResourceType { get; }
    public List<QueryParameter> QueryParameters { get; }
    public DomainResource Resource { get; }
    public string? ForVersion { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? Preference { get; }
}

public class CdrPatchByIdRequest
{
    public CdrPatchByIdRequest(
        string resourceType,
        string id,
        List<Operation> operations,
        string? forVersion = null,
        bool? shouldValidate = null,
        FormatParameter? format = null,
        ReturnPreference? preference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(id));

        ResourceType = resourceType;
        Id = id;
        Operations = operations;
        ForVersion = forVersion;
        ShouldValidate = shouldValidate;
        Format = format;
        Preference = preference;
    }

    public string ResourceType { get; }
    public string Id { get; }
    public List<Operation> Operations { get; }
    public string? ForVersion { get; }
    public bool? ShouldValidate { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? Preference { get; }
}

public class CdrPatchByQueryRequest
{
    public CdrPatchByQueryRequest(
        string resourceType,
        List<QueryParameter> queryParameters,
        List<Operation> operations,
        string? forVersion = null,
        FormatParameter? format = null,
        ReturnPreference? preference = null
    )
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNull(queryParameters, nameof(queryParameters));

        ResourceType = resourceType;
        QueryParameters = queryParameters;
        Operations = operations;
        ForVersion = forVersion;
        Format = format;
        Preference = preference;
    }

    public string ResourceType { get; }
    public List<QueryParameter> QueryParameters { get; }
    public List<Operation> Operations { get; }
    public string? ForVersion { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? Preference { get; }
}

#endregion

#region Responses

public class CdrReadResponse
{
    public int Status { get; set; }
    public DomainResource? Resource { get; set; }
}

public class CdrSearchResponse
{
    public int Status { get; set; }
    public Bundle? Bundle { get; set; }
    public OperationOutcome? OperationOutcome { get; set; }
}

public class CdrCreateResponse
{
    public int Status { get; set; }
    public DomainResource? Resource { get; set; }
    public string Location { get; set; }
    public string ETag { get; set; }
    public string LastModified { get; set; }
}   

public class CdrBatchOrTransactionResponse
{
    public int Status { get; set; }
    public Bundle? Bundle { get; set; }
    public OperationOutcome? OperationOutcome { get; set; }
}

public class CdrDeleteResponse
{
    public int Status { get; set; }
    public OperationOutcome? OperationOutcome { get; set; }
}

public class CdrUpdateResponse
{
    public int Status { get; set; }
    public DomainResource? Resource { get; set; }
    public string Location { get; set; }
    public string ETag { get; set; }
    public string LastModified { get; set; }
}

public class CdrPatchResponse
{
    public int Status { get; set; }
    public DomainResource? Resource { get; set; }
    public string Location { get; set; }
    public string ETag { get; set; }
    public string LastModified { get; set; }
}

#endregion
