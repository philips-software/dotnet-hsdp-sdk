using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.TDR.Internal;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR;

public class HsdpTdr : IHsdpTdr
{
    private readonly IRequestFactory _requestFactory;
    private readonly IHttpRequester _http;
    private const int DefaultLimit = 100;

    public HsdpTdr(HsdpTdrConfiguration configuration)
        : this(new HttpRequester(), new RequestFactory(configuration))
    {
    }

    internal HsdpTdr(IHttpRequester http, IRequestFactory requestFactory)
    {
        _http = http;
        _requestFactory = requestFactory;
    }

    public async Task<IHsdpResponse<DataItems>> SearchDataItems(
        TdrSearchDataRequestByUrl searchDataRequest,
        IIamToken token,
        string? requestId
    )
    {
        var request = _requestFactory.CreateSearchDataItemByUrlRequest(searchDataRequest, token, requestId);
        var response = await _http.HttpRequest<BundleForGetDataItemResponse>(request);
        return CreateDataItemsResponse(response);
    }

    public async Task<IHsdpResponse<DataItems>> SearchDataItems(
        TdrSearchDataRequest searchDataRequest,
        IIamToken token,
        string? requestId
    )
    {
        var request = _requestFactory.CreateSearchDataItemRequest(searchDataRequest, token, requestId);
        var response = await _http.HttpRequest<BundleForGetDataItemResponse>(request);
        return CreateDataItemsResponse(response);
    }

    public Task StoreDataItem(
        TdrStoreDataRequest storeDataRequest,
        IIamToken token,
        string? requestId = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDataItem(
        TdrDeleteDataRequest deleteDataRequest,
        IIamToken token,
        string? requestId = null
    )
    {
        throw new NotImplementedException();
    }

    public Task PatchDataItem(
        TdrPatchDataRequest patchDataRequest,
        IIamToken token,
        string? requestId = null
    )
    {
        throw new NotImplementedException();
    }

    public Task StoreDataItems(
        TdrStoreDataBatchRequest storeDataRequest,
        IIamToken token,
        string? requestId = null
    )
    {
        throw new NotImplementedException();
    }

    private static IHsdpResponse<DataItems> CreateDataItemsResponse(IHsdpResponse<BundleForGetDataItemResponse> response)
    {
        var bundle = response.Body;
        if (bundle == null) throw new Exception("Data item search failed");
        var limit = DetermineLimitFromResponseLink(bundle);
        var fakeRequestId = Guid.NewGuid().ToString(); // TODO: parse response headers to get the request id
        return new HsdpResponse<DataItems>(
            FilterResponseHeaders(response.Headers, new List<string>
            {
                "ETag"
            }),
            new DataItems(
                bundle.entry.Select(CreateDataItem).ToList(),
                new Pagination(bundle._startAt, limit),
                fakeRequestId
            )
        );
    }

    private static List<KeyValuePair<string,string>> FilterResponseHeaders(
        IEnumerable<KeyValuePair<string, string>> responseHeaders,
        ICollection<string> wantedHeaders
    )
    {
        return responseHeaders.Where(header => wantedHeaders.Contains(header.Key)).ToList();
    }

    private static int DetermineLimitFromResponseLink(BundleForGetDataItemResponse response)
    {
        var link = response.link.FirstOrDefault();
        var limit = link != null ? 1 : DefaultLimit; // TODO: use regex to extract the limit value
        return limit;
    }

    private static DataItem CreateDataItem(BundleEntryDataItem item)
    {
        return new DataItem(item.resource.timestamp, CreateDataType(item.resource.dataType), item.resource.organization)
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

    private static Coding CreateDataType(Internal.Coding dataType)
    {
        return new Coding(dataType.system, dataType.code);
    }

    private static Meta? CreateMeta(Internal.Meta? meta)
    {
        return meta != null ? new Meta(meta.lastUpdated, meta.versionId) : null;
    }

    private static Identifier? CreateIdentifier(Internal.Identifier? identifier)
    {
        return identifier != null ? new Identifier(identifier.system, identifier.value) : null;
    }

    private static Blob? CreateBlob(string? blob)
    {
        return blob != null ? new Blob(blob.DecodeBase64()) : null;
    }
}