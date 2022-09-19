using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.TDR;
using DotnetHsdpSdk.TDR.Internal;
using DotnetHsdpSdk.Utils;
using Moq;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.TDR;

[TestFixture]
public class HsdpTdrTests
{
    
    private HsdpTdr _tdr = null!;
    private Mock<IHttpRequester> _http = null!;
    private Mock<IHsdpRequestFactory> _requestFactory = null!;
    private Mock<ITdrResponseFactory> _responseFactory = null!;
    private Mock<IHsdpRequest> _hsdpRequest = null!;
    private Mock<IHsdpResponse<string>> _hsdpResponse = null!;
    private Mock<IIamToken> _token = null!;

    [SetUp]
    public void SetUp()
    {
        _http = new Mock<IHttpRequester>();
        _requestFactory = new Mock<IHsdpRequestFactory>();
        _responseFactory = new Mock<ITdrResponseFactory>();
        _token = new Mock<IIamToken>();
        _hsdpRequest = new Mock<IHsdpRequest>();
        _hsdpResponse = new Mock<IHsdpResponse<string>>();
        
        _tdr = new HsdpTdr(_http.Object, _requestFactory.Object, _responseFactory.Object);
    }

    [Test]
    public async Task SearchDataItemsByFullUrlShouldCallInvolvedStepsAndReturnTheMockedResponse()
    {
        var request = new Mock<TdrSearchDataByUrlRequest>("http://fullUrl.com");
        var response = new Mock<TdrSearchDataResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object, null))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateTdrSearchDataResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _tdr.SearchDataItems(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object, null));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateTdrSearchDataResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }
    
    [Test]
    public async Task SearchDataItemsByQueryShouldCallInvolvedStepsAndReturnTheMockedResponse()
    {
        var request = new Mock<TdrSearchDataByQueryRequest>(new List<KeyValuePair<string, string>>());
        var response = new Mock<TdrSearchDataResponse>();
        _requestFactory
            .Setup(f => f.Create(request.Object, _token.Object, null))
            .Returns(_hsdpRequest.Object);
        _http
            .Setup(h => h.HttpRequest<string>(_hsdpRequest.Object))
            .ReturnsAsync(_hsdpResponse.Object);
        _responseFactory
            .Setup(f => f.CreateTdrSearchDataResponse(_hsdpResponse.Object))
            .Returns(response.Object);

        var result = await _tdr.SearchDataItems(request.Object, _token.Object);
        
        _requestFactory.Verify(h => h.Create(request.Object, _token.Object, null));
        _http.Verify(h => h.HttpRequest<string>(_hsdpRequest.Object));
        _responseFactory.Verify(h => h.CreateTdrSearchDataResponse(_hsdpResponse.Object));
        Assert.AreEqual(response.Object, result);
    }
}
