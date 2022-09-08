using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DotnetHsdpSdk.TDR.Internal;

internal class BundleForGetDataItemResponse
{
#pragma warning disable IDE1006 // Naming Styles

    public string resourceType { get; set; }
    public string type { get; set; }
    public int total { get; set; }
    [JsonPropertyName("_startAt")] public int _startAt { get; set; }
    public List<Link> link { get; set; }
    public List<BundleEntryDataItem> entry { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}

internal class Link
{
#pragma warning disable IDE1006 // Naming Styles

    public string relation { get; set; }
    public string url { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}

internal class BundleEntryDataItem
{
#pragma warning disable IDE1006 // Naming Styles

    public string fullUrl { get; set; }
    public DataItem resource { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}

internal class DataItem
{
#pragma warning disable IDE1006 // Naming Styles

    public string? id { get; set; }
    public Meta? meta { get; set; }
    public string? resourceType { get; set; }
    public string timestamp { get; set; }
    public int? sequenceNumber { get; set; }
    public Identifier? device { get; set; }
    public Identifier? user { get; set; }
    public Identifier? relatedPeripheral { get; set; }
    public Identifier? relatedUser { get; set; }
    public Coding dataType { get; set; }
    public string organization { get; set; }
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

#pragma warning restore IDE1006 // Naming Styles
}

public class Meta
{
#pragma warning disable IDE1006 // Naming Styles

    public string lastUpdated { get; set; }
    public string versionId { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}

public class Identifier
{
#pragma warning disable IDE1006 // Naming Styles

    public string? system { get; set; }
    public string value { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}

public class Coding
{
#pragma warning disable IDE1006 // Naming Styles

    public string system { get; set; }
    public string code { get; set; }

#pragma warning restore IDE1006 // Naming Styles
}
