using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR.Internal;

public interface ITdrResponseFactory
{
    TdrSearchDataResponse CreateTdrSearchDataResponse(IHsdpResponse<string> response);
}

internal class TdrResponseFactory : ITdrResponseFactory
{
    private const int DefaultLimit = 100;

    public TdrSearchDataResponse CreateTdrSearchDataResponse(IHsdpResponse<string> hsdpResponse)
    {
        if (!hsdpResponse.StatusCode.IsSuccess())
            throw new HsdpRequestException(hsdpResponse.StatusCode, hsdpResponse.Body);

        var response = Deserialize<BundleForGetDataItemResponse>(hsdpResponse.Body);
        if (response.entry == null)
            throw new HsdpRequestException(InternalApiError.StatusCode, "Response has no entries");
        var requestId = hsdpResponse.Headers.Find(header => header.Key.ToLower() == "hsdp-request-id").Value;
        return new TdrSearchDataResponse
        {
            Status = hsdpResponse.StatusCode,
            DataItems = response.entry.Select(CreateDataItem).ToList(),
            Pagination = GetPaginationFromResponseLink(response),
            RequestId = requestId
        };
    }

    private static Pagination GetPaginationFromResponseLink(BundleForGetDataItemResponse response)
    {
        var offset = 0;
        var limit = DefaultLimit;
        var link = response.link?.FirstOrDefault(link => link.relation == "next");
        if (link != null)
        {
            limit = ExtractMatch(link, @"_count=(\d+)", DefaultLimit);
            offset = ExtractMatch(link, @"_startAt=(\d+)", 0);
        }

        return new Pagination(offset, limit);
    }

    private static int ExtractMatch(Link link, string pattern, int defaultValue)
    {
        var value = defaultValue;
        var match = Regex.Match(link.url, pattern);
        if (match.Success) value = int.Parse(match.Groups[1].Value);

        return value;
    }

    private static TDR.DataItem CreateDataItem(BundleEntryDataItem item)
    {
        return new TDR.DataItem(item.resource.timestamp, CreateDataType(item.resource.dataType),
            item.resource.organization)
        {
            Id = item.resource.id,
            Meta = CreateMeta(item.resource.meta),
            SequenceNumber = item.resource.sequenceNumber,
            Device = CreateIdentifier(item.resource.device),
            User = CreateIdentifier(item.resource.user),
            RelatedPeripheral = CreateIdentifier(item.resource.relatedPeripheral),
            RelatedUser = CreateIdentifier(item.resource.relatedUser),
            Application = item.resource.application,
            Proposition = item.resource.proposition,
            Subscription = item.resource.subscription,
            DataSource = item.resource.dataSource,
            DataCategory = item.resource.dataCategory,
            Data = item.resource.data,
            Blob = CreateBlob(item.resource.blob),
            DeleteTimestamp = item.resource.deleteTimestamp,
            CreationTimestamp = item.resource.creationTimestamp,
            Tombstone = item.resource.tombstone,
            Link = new SelfLink(item.fullUrl)
        };
    }

    private static T Deserialize<T>(string? body)
    {
        if (body == null) throw new HsdpRequestException(InternalApiError.StatusCode, "Response body is missing");
        try
        {
            var resource = JsonSerializer.Deserialize<T>(body);
            if (resource == null)
                throw new HsdpRequestException(InternalApiError.StatusCode, "Deserialized body is null");

            return resource;
        }
        catch (JsonException e)
        {
            throw new HsdpRequestException(InternalApiError.StatusCode, "Invalid JSON", e);
        }
    }

    private static TDR.Coding CreateDataType(Coding dataType)
    {
        return new TDR.Coding(dataType.system, dataType.code);
    }

    private static TDR.Meta? CreateMeta(Meta? meta)
    {
        return meta != null ? new TDR.Meta(meta.lastUpdated, meta.versionId) : null;
    }

    private static TDR.Identifier? CreateIdentifier(Identifier? identifier)
    {
        return identifier != null ? new TDR.Identifier(identifier.system, identifier.value) : null;
    }

    private static Blob? CreateBlob(string? blob)
    {
        return blob != null ? new Blob(blob.DecodeBase64()) : null;
    }
}
