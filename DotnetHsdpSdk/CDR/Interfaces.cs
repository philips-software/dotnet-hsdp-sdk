using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;

namespace DotnetHsdpSdk.CDR;

public interface IHsdpCdr
{
    Task<ReadResponse> Read(CdrReadRequest readRequest, IIamToken token);
    Task<ReadResponse> Read(CdrReadVersionRequest readVersionRequest, IIamToken token);
    Task<SearchResponse> Search(CdrSearchRequest searchRequest, IIamToken token);
    Task<CreateResponse> Create(CdrCreateRequest createRequest, IIamToken token);
    Task<BatchOrTransactionResponse> CreateBatchOrTransaction(CdrCreateBatchOrTransactionRequest createBatchOrTransactionRequest, IIamToken token);
    Task<DeleteResponse> Delete(CdrDeleteByIdRequest deleteRequest, IIamToken token);
    Task<DeleteResponse> Delete(CdrDeleteByQueryRequest deleteRequest, IIamToken token);
    Task<UpdateResponse> Update(CdrUpdateByIdRequest updateRequest, IIamToken token);
    Task<UpdateResponse> Update(CdrUpdateByQueryRequest updateRequest, IIamToken token);
    Task<PatchResponse> Patch(CdrPatchByIdRequest patchRequest, IIamToken token);
    Task<PatchResponse> Patch(CdrPatchByQueryRequest patchRequest, IIamToken token);
}
