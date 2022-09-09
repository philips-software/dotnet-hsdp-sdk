using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR;

public class HsdpTdrConfiguration
{
    public HsdpTdrConfiguration(Uri tdrEndpoint)
    {
        Validate.NotNull(tdrEndpoint, nameof(tdrEndpoint));

        TdrEndpoint = tdrEndpoint;
    }

    public Uri TdrEndpoint { get; }
}

public class TdrSearchDataRequestByUrl
{
    public TdrSearchDataRequestByUrl(string fullUrl)
    {
        Validate.NotNullOrEmpty(fullUrl, nameof(fullUrl));

        FullUrl = fullUrl;
    }

    public string FullUrl { get; }
}

public class TdrSearchDataRequest
{
    public TdrSearchDataRequest(List<KeyValuePair<string, string>> queryParameters)
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

public class DataItems
{
    public List<DataItem> Data { get; set; }
    public Pagination Pagination { get; set; }
    public string RequestId { get; set; }

    public DataItems(List<DataItem> data, Pagination pagination, string requestId)
    {
        Data = data;
        Pagination = pagination;
        RequestId = requestId;
    }
}

public class DataItem
{
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

    public DataItem(string timestamp, Coding dataType, string organization)
    {
        Timestamp = timestamp;
        DataType = dataType;
        Organization = organization;
    }
}

public class Meta
{
    public string LastUpdated { get; set; }
    public string VersionId { get; set; }

    public Meta(string lastUpdated, string versionId)
    {
        LastUpdated = lastUpdated;
        VersionId = versionId;
    }
}

public class Identifier
{
    public string? System { get; set; }
    public string Value { get; set; }

    public Identifier(string? system, string value)
    {
        System = system;
        Value = value;
    }
}

public class Coding
{
    public string System { get; set; }
    public string Code { get; set; }

    public Coding(string system, string code)
    {
        System = system;
        Code = code;
    }
}

public class Blob
{
    public byte[] Data { get; set; }

    public Blob(byte[] data)
    {
        Data = data;
    }
}

public class SelfLink
{
    public string Self { get; set; }

    public SelfLink(string self)
    {
        Self = self;
    }
}

public class Pagination
{
    public int Offset { get; set; }
    public int Limit { get; set; }

    public Pagination(int offset, int limit)
    {
        Offset = offset;
        Limit = limit;
    }
}

