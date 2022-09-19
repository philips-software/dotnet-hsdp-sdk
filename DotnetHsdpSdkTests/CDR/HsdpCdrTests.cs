using DotnetHsdpSdk.CDR;
using DotnetHsdpSdk.CDR.Internal;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using NUnit.Framework;
using Task = System.Threading.Tasks.Task;

namespace DotnetHsdpSdkTests.CDR;

[TestFixture]
public class HsdpCdrTests
{
    private HsdpCdr _cdr = null!;
    private Mock<IHttpRequester> _http = null!;
    private Mock<IHsdpRequestFactory> _requestFactory = null!;
    private Mock<ICdrResponseFactory> _responseFactory = null!;
    private Mock<IHsdpRequest> _hsdpRequest = null!;
    private Mock<IHsdpResponse<string>> _hsdpResponse = null!;
    private Mock<IIamToken> _token = null!;

    [SetUp]
    public void SetUp()
    {
        _http = new Mock<IHttpRequester>();
        _requestFactory = new Mock<IHsdpRequestFactory>();
        _responseFactory = new Mock<ICdrResponseFactory>();
        _token = new Mock<IIamToken>();
        _hsdpRequest = new Mock<IHsdpRequest>();
        _hsdpResponse = new Mock<IHsdpResponse<string>>();
        
        _cdr = new HsdpCdr(_http.Object, _requestFactory.Object, _responseFactory.Object);
    }

    [Test]
    public async Task ReadShouldReturnACdrReadResponse()
    {
        var request = new Mock<CdrReadRequest>("Observation", "ObservationId", null, null);
        var response = new Mock<CdrReadResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrReadResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Read(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrReadResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task ReadVersionShouldReturnACdrReadResponse()
    {
        var request = new Mock<CdrReadVersionRequest>("Observation", "ObservationId", "VersionId");
        var response = new Mock<CdrReadResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrReadResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Read(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrReadResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task SearchShouldReturnACdrSearchResponse()
    {
        var request = new Mock<CdrSearchRequest>("Observation", SearchMethod.Get, null, null);
        var response = new Mock<CdrSearchResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrSearchResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Search(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrSearchResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task CreateShouldReturnACdrCreateResponse()
    {
        var observation = new Mock<Observation>();
        var request = new Mock<CdrCreateRequest>("Observation", observation.Object, null, null, null);
        var response = new Mock<CdrCreateResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrCreateResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Create(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrCreateResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task BatchOrTransactionShouldReturnACdrBatchOrTransactionResponse()
    {
        var bundle = new Mock<Bundle>();
        var request = new Mock<CdrBatchOrTransactionRequest>(bundle.Object, null);
        var response = new Mock<CdrBatchOrTransactionResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrBatchOrTransactionResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.BatchOrTransaction(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrBatchOrTransactionResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task DeleteByIdShouldReturnACdrDeleteResponse()
    {
        var request = new Mock<CdrDeleteByIdRequest>("Observation", "ObservationId");
        var response = new Mock<CdrDeleteResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrDeleteResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Delete(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrDeleteResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task DeleteByQueryShouldReturnACdrDeleteResponse()
    {
        var request = new Mock<CdrDeleteByQueryRequest>("Observation", new List<QueryParameter>());
        var response = new Mock<CdrDeleteResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrDeleteResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Delete(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrDeleteResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task UpdateByIdShouldReturnACdrDeleteResponse()
    {
        var resource = new Mock<Observation>();
        var request = new Mock<CdrUpdateByIdRequest>("Observation", "ObservationId", resource.Object, null, null, null);
        var response = new Mock<CdrUpdateResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrUpdateResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Update(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrUpdateResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task UpdateByQueryShouldReturnACdrDeleteResponse()
    {
        var resource = new Mock<Observation>();
        var request = new Mock<CdrUpdateByQueryRequest>("Observation", new List<QueryParameter>(), resource.Object, null, null);
        var response = new Mock<CdrUpdateResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrUpdateResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Update(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrUpdateResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task PatchByIdShouldReturnACdrDeleteResponse()
    {
        var request = new Mock<CdrPatchByIdRequest>("Observation", "ObservationId", new List<Operation>(), null, null, null);
        var response = new Mock<CdrPatchResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrPatchResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Patch(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrPatchResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }

    [Test]
    public async Task PatchByQueryShouldReturnACdrDeleteResponse()
    {
        var request = new Mock<CdrPatchByQueryRequest>("Observation", new List<QueryParameter>(), new List<Operation>(), null, null);
        var response = new Mock<CdrPatchResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateCdrPatchResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _cdr.Patch(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateCdrPatchResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }
}
