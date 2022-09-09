using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR;

public interface IHsdpTdr
{
    Task<IHsdpResponse<DataItems>> SearchDataItems(
        TdrSearchDataRequestByUrl searchDataRequest,
        IIamToken token,
        string? requestId = null
    );
    Task<IHsdpResponse<DataItems>> SearchDataItems(
        TdrSearchDataRequest searchDataRequest,
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
