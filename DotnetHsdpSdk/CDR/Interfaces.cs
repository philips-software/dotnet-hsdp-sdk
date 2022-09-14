using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;

namespace DotnetHsdpSdk.CDR;

public interface IHsdpCdr
{
    Task<CdrReadResponse> Read(CdrReadRequest readRequest, IIamToken token);
    Task<CdrReadResponse> Read(CdrReadVersionRequest readVersionRequest, IIamToken token);
    Task<CdrSearchResponse> Search(CdrSearchRequest searchRequest, IIamToken token);
    Task<CdrCreateResponse> Create(CdrCreateRequest createRequest, IIamToken token);
    Task<CdrBatchOrTransactionResponse> BatchOrTransaction(CdrBatchOrTransactionRequest batchOrTransactionRequest, IIamToken token);
    Task<CdrDeleteResponse> Delete(CdrDeleteByIdRequest deleteRequest, IIamToken token);
    Task<CdrDeleteResponse> Delete(CdrDeleteByQueryRequest deleteRequest, IIamToken token);
    Task<CdrUpdateResponse> Update(CdrUpdateByIdRequest updateRequest, IIamToken token);
    Task<CdrUpdateResponse> Update(CdrUpdateByQueryRequest updateRequest, IIamToken token);
    Task<CdrPatchResponse> Patch(CdrPatchByIdRequest patchRequest, IIamToken token);
    Task<CdrPatchResponse> Patch(CdrPatchByQueryRequest patchRequest, IIamToken token);
}
