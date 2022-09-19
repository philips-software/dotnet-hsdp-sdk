using System.Threading.Tasks;
using DotnetHsdpSdk.CDR.Internal;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.CDR;

public class HsdpCdr : IHsdpCdr
{
    private readonly ICdrResponseFactory _cdrResponseFactory;
    private readonly IHsdpRequestFactory _hsdpRequestFactory;
    private readonly IHttpRequester _http;

    public HsdpCdr(HsdpCdrConfiguration configuration)
        : this(new HttpRequester(), new HsdpRequestFactory(configuration), new CdrResponseFactory())
    {
    }

    internal HsdpCdr(IHttpRequester http, IHsdpRequestFactory hsdpRequestFactory,
        ICdrResponseFactory cdrResponseFactory)
    {
        _http = http;
        _hsdpRequestFactory = hsdpRequestFactory;
        _cdrResponseFactory = cdrResponseFactory;
    }

    public async Task<CdrReadResponse> Read(CdrReadRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrReadResponse(hsdpResponse);
    }

    public async Task<CdrReadResponse> Read(CdrReadVersionRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrReadResponse(hsdpResponse);
    }

    public async Task<CdrSearchResponse> Search(CdrSearchRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrSearchResponse(hsdpResponse);
    }

    public async Task<CdrCreateResponse> Create(CdrCreateRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrCreateResponse(hsdpResponse);
    }

    public async Task<CdrBatchOrTransactionResponse> BatchOrTransaction(CdrBatchOrTransactionRequest request,
        IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrBatchOrTransactionResponse(hsdpResponse);
    }

    public async Task<CdrDeleteResponse> Delete(CdrDeleteByIdRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrDeleteResponse(hsdpResponse);
    }

    public async Task<CdrDeleteResponse> Delete(CdrDeleteByQueryRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrDeleteResponse(hsdpResponse);
    }

    public async Task<CdrUpdateResponse> Update(CdrUpdateByIdRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrUpdateResponse(hsdpResponse);
    }

    public async Task<CdrUpdateResponse> Update(CdrUpdateByQueryRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrUpdateResponse(hsdpResponse);
    }

    public async Task<CdrPatchResponse> Patch(CdrPatchByIdRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrPatchResponse(hsdpResponse);
    }

    public async Task<CdrPatchResponse> Patch(CdrPatchByQueryRequest request, IIamToken token)
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _cdrResponseFactory.CreateCdrPatchResponse(hsdpResponse);
    }
}
