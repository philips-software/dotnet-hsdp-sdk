using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;

namespace DotnetHsdpSdk.TDR;

public interface IHsdpTdr
{
    Task<TdrSearchDataResponse> SearchDataItems(
        TdrSearchDataByUrlRequest request,
        IIamToken token,
        string? requestId = null
    );

    Task<TdrSearchDataResponse> SearchDataItems(
        TdrSearchDataByQueryRequest request,
        IIamToken token,
        string? requestId = null
    );

    Task StoreDataItem(TdrStoreDataRequest request, IIamToken token, string? requestId = null);
    Task DeleteDataItem(TdrDeleteDataRequest request, IIamToken token, string? requestId = null);
    Task PatchDataItem(TdrPatchDataRequest request, IIamToken token, string? requestId = null);
    Task StoreDataItems(TdrStoreDataBatchRequest request, IIamToken token, string? requestId = null);
}
