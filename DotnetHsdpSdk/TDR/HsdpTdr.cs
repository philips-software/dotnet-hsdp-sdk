using System;
using System.Threading.Tasks;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.TDR.Internal;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.TDR;

public class HsdpTdr : IHsdpTdr
{
    private readonly IHsdpRequestFactory _hsdpRequestFactory;
    private readonly IHttpRequester _http;
    private readonly ITdrResponseFactory _tdrResponseFactory;

    public HsdpTdr(HsdpTdrConfiguration configuration)
        : this(new HttpRequester(), new HsdpRequestFactory(configuration), new TdrResponseFactory())
    {
    }

    internal HsdpTdr(IHttpRequester http, IHsdpRequestFactory hsdpRequestFactory,
        ITdrResponseFactory tdrResponseFactory)
    {
        _http = http;
        _hsdpRequestFactory = hsdpRequestFactory;
        _tdrResponseFactory = tdrResponseFactory;
    }

    public async Task<TdrSearchDataResponse> SearchDataItems(
        TdrSearchDataByUrlRequest request,
        IIamToken token,
        string? requestId = null
    )
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token, requestId);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _tdrResponseFactory.CreateTdrSearchDataResponse(hsdpResponse);
    }

    public async Task<TdrSearchDataResponse> SearchDataItems(
        TdrSearchDataByQueryRequest request,
        IIamToken token,
        string? requestId = null
    )
    {
        var hsdpRequest = _hsdpRequestFactory.Create(request, token, requestId);
        var hsdpResponse = await _http.HttpRequest<string>(hsdpRequest);
        return _tdrResponseFactory.CreateTdrSearchDataResponse(hsdpResponse);
    }

    public Task StoreDataItem(TdrStoreDataRequest request, IIamToken token, string? requestId = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDataItem(TdrDeleteDataRequest request, IIamToken token, string? requestId = null)
    {
        throw new NotImplementedException();
    }

    public Task PatchDataItem(TdrPatchDataRequest request, IIamToken token, string? requestId = null)
    {
        throw new NotImplementedException();
    }

    public Task StoreDataItems(TdrStoreDataBatchRequest request, IIamToken token, string? requestId = null)
    {
        throw new NotImplementedException();
    }
}
