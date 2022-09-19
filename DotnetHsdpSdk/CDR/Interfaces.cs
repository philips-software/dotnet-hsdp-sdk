using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;

namespace DotnetHsdpSdk.CDR;

public interface IHsdpCdr
{
    Task<CdrReadResponse> Read(CdrReadRequest request, IIamToken token);
    Task<CdrReadResponse> Read(CdrReadVersionRequest request, IIamToken token);
    Task<CdrSearchResponse> Search(CdrSearchRequest request, IIamToken token);
    Task<CdrCreateResponse> Create(CdrCreateRequest request, IIamToken token);
    Task<CdrBatchOrTransactionResponse> BatchOrTransaction(CdrBatchOrTransactionRequest request, IIamToken token);
    Task<CdrDeleteResponse> Delete(CdrDeleteByIdRequest request, IIamToken token);
    Task<CdrDeleteResponse> Delete(CdrDeleteByQueryRequest request, IIamToken token);
    Task<CdrUpdateResponse> Update(CdrUpdateByIdRequest request, IIamToken token);
    Task<CdrUpdateResponse> Update(CdrUpdateByQueryRequest request, IIamToken token);
    Task<CdrPatchResponse> Patch(CdrPatchByIdRequest request, IIamToken token);
    Task<CdrPatchResponse> Patch(CdrPatchByQueryRequest request, IIamToken token);
}
