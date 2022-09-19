using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DotnetHsdpSdk.TDR.Internal;

#pragma warning disable IDE1006 // Naming Styles
internal class BundleForGetDataItemResponse
{
    public string resourceType { get; set; }
    public string type { get; set; }
    public int total { get; set; }
    [JsonPropertyName("_startAt")] public int _startAt { get; set; }
    public List<Link> link { get; set; }
    public List<BundleEntryDataItem> entry { get; set; }
}

internal class Link
{
    public string relation { get; set; }
    public string url { get; set; }
}

internal class BundleEntryDataItem
{
    public string fullUrl { get; set; }
    public DataItem resource { get; set; }
}

internal class DataItem
{
    public DataItem(string timestamp, Coding dataType, string organization)
    {
        this.timestamp = timestamp;
        this.dataType = dataType;
        this.organization = organization;
    }

    public string? id { get; set; }
    public Meta? meta { get; set; }
    public string? resourceType { get; set; }
    public string timestamp { get; }
    public int? sequenceNumber { get; set; }
    public Identifier? device { get; set; }
    public Identifier? user { get; set; }
    public Identifier? relatedPeripheral { get; set; }
    public Identifier? relatedUser { get; set; }
    public Coding dataType { get; }
    public string organization { get; }
    public string? application { get; set; }
    public string? proposition { get; set; }
    public string? subscription { get; set; }
    public string? dataSource { get; set; }
    public string? dataCategory { get; set; }
    public JsonObject? data { get; set; }
    public string? blob { get; set; }
    public string? deleteTimestamp { get; set; }
    public string? creationTimestamp { get; set; }
    public bool? tombstone { get; set; }
}

public class Meta
{
    public Meta(string lastUpdated, string versionId)
    {
        this.lastUpdated = lastUpdated;
        this.versionId = versionId;
    }

    public string lastUpdated { get; }
    public string versionId { get; }
}

public class Identifier
{
    public Identifier(string? system, string value)
    {
        this.system = system;
        this.value = value;
    }

    public string? system { get; }
    public string value { get; }
}

public class Coding
{
    public Coding(string system, string code)
    {
        this.system = system;
        this.code = code;
    }

    public string system { get; }
    public string code { get; }
}
#pragma warning restore IDE1006 // Naming Styles
