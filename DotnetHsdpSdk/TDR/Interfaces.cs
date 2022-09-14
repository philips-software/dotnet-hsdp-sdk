using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR;

public interface IHsdpTdr
{
    Task<TdrSearchDataResponse> SearchDataItems(
        TdrSearchDataByUrlRequest searchDataByUrlRequest,
        IIamToken token,
        string? requestId = null
    );
    Task<TdrSearchDataResponse> SearchDataItems(
        TdrSearchDataByQueryRequest searchDataByQueryRequest,
        IIamToken token,
        string? requestId = null
    );
    Task StoreDataItem(
        TdrStoreDataRequest storeDataRequest,
        IIamToken token,
        string? requestId = null
    );
    Task DeleteDataItem(
        TdrDeleteDataRequest deleteDataRequest,
        IIamToken token,
        string? requestId = null
    );
    Task PatchDataItem(
        TdrPatchDataRequest patchDataRequest,
        IIamToken token,
        string? requestId = null
    );
    Task StoreDataItems(
        TdrStoreDataBatchRequest storeDataRequest,
        IIamToken token,
        string? requestId = null
    );
}
