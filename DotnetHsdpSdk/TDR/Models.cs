using System.Collections.Generic;
using System.Text.Json.Nodes;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR;

public class HsdpTdrConfiguration
{
    public HsdpTdrConfiguration(string tdrEndpoint)
    {
        Validate.NotNull(tdrEndpoint, nameof(tdrEndpoint));

        TdrEndpoint = tdrEndpoint;
    }

    public string TdrEndpoint { get; }
}

#region Requests

public class TdrSearchDataByUrlRequest
{
    public TdrSearchDataByUrlRequest(string fullUrl)
    {
        Validate.NotNullOrEmpty(fullUrl, nameof(fullUrl));

        FullUrl = fullUrl;
    }

    public string FullUrl { get; }
}

public class TdrSearchDataByQueryRequest
{
    public TdrSearchDataByQueryRequest(List<KeyValuePair<string, string>> queryParameters)
    {
        QueryParameters = queryParameters;
    }

    public List<KeyValuePair<string, string>> QueryParameters { get; }
}

public class TdrStoreDataRequest
{
}

public class TdrDeleteDataRequest
{
}

public class TdrPatchDataRequest
{
}

public class TdrStoreDataBatchRequest
{
}

#endregion

#region Responses

public class TdrSearchDataResponse
{
    public int Status { get; set; }
    public List<DataItem> DataItems { get; set; }
    public Pagination Pagination { get; set; }
    public string RequestId { get; set; }
}

public class TdrStoreDataResponse
{
}

public class TdrDeleteDataResponse
{
}

public class TdrPatchDataResponse
{
}

public class TdrStoreDataBatchResponse
{
}

public class DataItem
{
    public DataItem(string timestamp, Coding dataType, string organization)
    {
        Timestamp = timestamp;
        DataType = dataType;
        Organization = organization;
    }

    public string? Id { get; set; }
    public Meta? Meta { get; set; }
    public string Timestamp { get; set; }
    public int? SequenceNumber { get; set; }
    public Identifier? Device { get; set; }
    public Identifier? User { get; set; }
    public Identifier? RelatedPeripheral { get; set; }
    public Identifier? RelatedUser { get; set; }
    public Coding DataType { get; set; }
    public string Organization { get; set; }
    public string? Application { get; set; }
    public string? Proposition { get; set; }
    public string? Subscription { get; set; }
    public string? DataSource { get; set; }
    public string? DataCategory { get; set; }

    /**
     * As [data] is a dynamic structure, it cannot be modelled with predefined classes.
     * Therefore, it is exposed as a JsonObject which can be (de)serialized (from)to JSON.
     */
    public JsonObject? Data { get; set; }

    public Blob? Blob { get; set; }
    public string? DeleteTimestamp { get; set; }
    public string? CreationTimestamp { get; set; }
    public bool? Tombstone { get; set; }
    public SelfLink? Link { get; set; }
}

public class Meta
{
    public Meta(string lastUpdated, string versionId)
    {
        LastUpdated = lastUpdated;
        VersionId = versionId;
    }

    public string LastUpdated { get; }
    public string VersionId { get; }
}

public class Identifier
{
    public Identifier(string? system, string value)
    {
        System = system;
        Value = value;
    }

    public string? System { get; }
    public string Value { get; }
}

public class Coding
{
    public Coding(string system, string code)
    {
        System = system;
        Code = code;
    }

    public string System { get; }
    public string Code { get; }
}

public class Blob
{
    public Blob(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}

public class SelfLink
{
    public SelfLink(string self)
    {
        Self = self;
    }

    public string Self { get; }
}

public class Pagination
{
    public Pagination(int offset, int limit)
    {
        Offset = offset;
        Limit = limit;
    }

    public int Offset { get; }
    public int Limit { get; }
}

#endregion
