using System;
using System.Collections.Generic;
using DotnetHsdpSdk.Utils;

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
    public CdrReadRequest(string resourceType)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
    
        ResourceType = resourceType;
    }

    public CdrReadRequest(string resourceType, string id, string? modifiedSinceTimestamp, string? modifiedSinceVersion, bool? pretty)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(resourceType));

        ResourceType = resourceType;
        Id = id;
        ModifiedSinceTimestamp = modifiedSinceTimestamp;
        ModifiedSinceVersion = modifiedSinceVersion;
        Pretty = pretty;
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
    public CdrReadVersionRequest(string resourceType, string id, string versionId)
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
    public CdrSearchRequest(Compartment compartment, string resourceType, SearchMethod method, List<QueryParameter>? queryParameters)
    {
        Validate.NotNull(compartment, nameof(compartment));
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        
        Compartment = compartment;
        ResourceType = resourceType;
        QueryParameters = queryParameters;
        Method = method;
    }

    public Compartment Compartment { get; }
    public string ResourceType { get; }
    public SearchMethod Method { get; }
    public FormatParameter? Format { get; }
    public List<QueryParameter>? QueryParameters;
}

public class QueryParameter
{
    public string Name { get; }

    public QueryParameter(string name, string value)
    {
        Validate.NotNull(name, nameof(name));
        Validate.NotNullOrEmpty(value, nameof(value));

        Name = name;
        Value = value;
    }

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

public enum SearchMethod {
    Get,
    Post,
}

public class CdrCreateRequest
{
    public CdrCreateRequest(string resourceType, string body, bool? shouldValidate, string? condition, ReturnPreference? preference)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(body, nameof(body));

        ResourceType = resourceType;
        Body = body;
        ShouldValidate = shouldValidate;
        Condition = condition;
        Preference = preference;
    }

    public string ResourceType { get; }
    public string Body { get; }
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

public class CdrCreateBatchOrTransactionRequest
{
    public CdrCreateBatchOrTransactionRequest(string body, ReturnPreference? preference)
    {
        Validate.NotNullOrEmpty(body, nameof(body));
        
        Body = body;
        Preference = preference;
    }

    public string Body { get; }
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
    public CdrUpdateByIdRequest(string resourceType, string id, string body, string? forVersion, bool? shouldValidate, ReturnPreference? preference)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(id));
        Validate.NotNullOrEmpty(body, nameof(body));

        ResourceType = resourceType;
        Id = id;
        Body = body;
        ForVersion = forVersion;
        ShouldValidate = shouldValidate;
        this.preference = preference;
    }
    
    public string ResourceType { get; }
    public string Id { get; }
    public string Body { get; }
    public string? ForVersion { get; }
    public bool? ShouldValidate { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? preference { get; }
}

public class CdrUpdateByQueryRequest
{
    public CdrUpdateByQueryRequest(string resourceType, List<QueryParameter> queryParameters, string body, string? forVersion, ReturnPreference? preference)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNull(queryParameters, nameof(queryParameters));
        Validate.NotNullOrEmpty(body, nameof(body));

        ResourceType = resourceType;
        QueryParameters = queryParameters;
        Body = body;
        ForVersion = forVersion;
        this.preference = preference;
    }
    
    public string ResourceType { get; }
    public List<QueryParameter> QueryParameters { get; }
    public string Body { get; }
    public string? ForVersion { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? preference { get; }
}

public class CdrPatchByIdRequest
{
    public CdrPatchByIdRequest(string resourceType, string id, string body, PatchContentType contentType, string? forVersion, bool? shouldValidate, ReturnPreference? preference)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNullOrEmpty(id, nameof(id));
        Validate.NotNullOrEmpty(body, nameof(body));

        ResourceType = resourceType;
        Id = id;
        Body = body;
        this.contentType = contentType;
        ForVersion = forVersion;
        ShouldValidate = shouldValidate;
        this.preference = preference;
    }
    
    public string ResourceType { get; }
    public string Id { get; }
    public string Body { get; }
    public PatchContentType contentType { get; }
    public string? ForVersion { get; }
    public bool? ShouldValidate { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? preference { get; }
}

public enum PatchContentType
{
    JsonPatchPlusJson,
    XmlPatchPlusJson,
    FhirXml,
    FhirJson,
}

public class CdrPatchByQueryRequest
{
    public CdrPatchByQueryRequest(string resourceType, List<QueryParameter> queryParameters, string body, PatchContentType contentType, string? forVersion, ReturnPreference? preference)
    {
        Validate.NotNullOrEmpty(resourceType, nameof(resourceType));
        Validate.NotNull(queryParameters, nameof(queryParameters));
        Validate.NotNullOrEmpty(body, nameof(body));

        ResourceType = resourceType;
        QueryParameters = queryParameters;
        Body = body;
        this.contentType = contentType;
        ForVersion = forVersion;
        this.preference = preference;
    }
    
    public string ResourceType { get; }
    public List<QueryParameter> QueryParameters { get; }
    public string Body { get; }
    public PatchContentType contentType { get; }
    public string? ForVersion { get; }
    public FormatParameter? Format { get; }
    public ReturnPreference? preference { get; }
}
#endregion

#region Responses

public class ReadResponse
{
}

public class SearchResponse
{
}

public class CreateResponse
{
}

public class BatchOrTransactionResponse
{
}

public class DeleteResponse
{
}


public class UpdateResponse
{
}

public class PatchResponse
{
}

#endregion

// public class DataItems
// {
//     public List<DataItem> Data { get; set; }
//     public Pagination Pagination { get; set; }
//     public string RequestId { get; set; }
//
//     public DataItems(List<DataItem> data, Pagination pagination, string requestId)
//     {
//         Data = data;
//         Pagination = pagination;
//         RequestId = requestId;
//     }
// }
//
// public class DataItem
// {
//     public string? Id { get; set; }
//     public Meta? Meta { get; set; }
//     public string Timestamp { get; set; }
//     public int? SequenceNumber { get; set; }
//     public Identifier? Device { get; set; }
//     public Identifier? User { get; set; }
//     public Identifier? RelatedPeripheral { get; set; }
//     public Identifier? RelatedUser { get; set; }
//     public Coding DataType { get; set; }
//     public string Organization { get; set; }
//     public string? Application { get; set; }
//     public string? Proposition { get; set; }
//     public string? Subscription { get; set; }
//     public string? DataSource { get; set; }
//     public string? DataCategory { get; set; }
//
//     /**
//      * As [data] is a dynamic structure, it cannot be modelled with predefined classes.
//      * Therefore, it is exposed as a JsonObject which can be (de)serialized (from)to JSON.
//      */
//     public JsonObject? Data { get; set; }
//     public Blob? Blob { get; set; }
//     public string? DeleteTimestamp { get; set; }
//     public string? CreationTimestamp { get; set; }
//     public bool? Tombstone { get; set; }
//     public SelfLink? Link { get; set; }
//
//     public DataItem(string timestamp, Coding dataType, string organization)
//     {
//         Timestamp = timestamp;
//         DataType = dataType;
//         Organization = organization;
//     }
// }
//
// public class Meta
// {
//     public string LastUpdated { get; set; }
//     public string VersionId { get; set; }
//
//     public Meta(string lastUpdated, string versionId)
//     {
//         LastUpdated = lastUpdated;
//         VersionId = versionId;
//     }
// }
//
// public class Identifier
// {
//     public string? System { get; set; }
//     public string Value { get; set; }
//
//     public Identifier(string? system, string value)
//     {
//         System = system;
//         Value = value;
//     }
// }
//
// public class Coding
// {
//     public string System { get; set; }
//     public string Code { get; set; }
//
//     public Coding(string system, string code)
//     {
//         System = system;
//         Code = code;
//     }
// }
//
// public class Blob
// {
//     public byte[] Data { get; set; }
//
//     public Blob(byte[] data)
//     {
//         Data = data;
//     }
// }
//
// public class SelfLink
// {
//     public string Self { get; set; }
//
//     public SelfLink(string self)
//     {
//         Self = self;
//     }
// }
//
// public class Pagination
// {
//     public int Offset { get; set; }
//     public int Limit { get; set; }
//
//     public Pagination(int offset, int limit)
//     {
//         Offset = offset;
//         Limit = limit;
//     }
// }
