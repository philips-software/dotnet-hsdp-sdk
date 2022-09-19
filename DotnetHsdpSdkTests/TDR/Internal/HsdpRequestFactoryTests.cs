using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.TDR;
using DotnetHsdpSdk.TDR.Internal;
using DotnetHsdpSdk.Utils;
using Moq;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.TDR.Internal;

[TestFixture]
public class HsdpRequestFactoryTests
{
    private const string TdrEndpoint = "https://endpoint.com";
    private readonly HsdpRequestFactory _hsdpRequestFactory = new(new HsdpTdrConfiguration(TdrEndpoint));
    private Mock<IIamToken> _token = null!;

    [SetUp]
    public void Setup()
    {
        _token = new Mock<IIamToken>();
        _token.Setup(f => f.AccessToken).Returns("accessToken");
    }

    #region SearchDataByURL

    [Test]
    public void CreationFromASearchDataByUrlShouldReturnAHsdpRequest()
    {
        var request = _hsdpRequestFactory.Create(new TdrSearchDataByUrlRequest(
            fullUrl: $"{TdrEndpoint}/store/tdr/DataItem?key1=value1?key2=value2"), 
            token: _token.Object
        );

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{TdrEndpoint}/store/tdr/DataItem?key1=value1?key2=value2"), request.Path);
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual(2, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "5"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
        });
    }

    [Test]
    public void CreationFromASearchDataByUrlWithRequestIdShouldReturnAHsdpRequestWithRequestIdHeader()
    {
        var request = _hsdpRequestFactory.Create(new TdrSearchDataByUrlRequest(
            fullUrl: $"{TdrEndpoint}/store/tdr/DataItem?key1=value1?key2=value2"), 
            token: _token.Object,
            requestId: "requestId"
        );

        Assert.Contains(KeyValuePair.Create("HSDP-Request-ID", "requestId"), request.Headers);
    }

    #endregion

    #region SearchDataByQuery

    [Test]
    public void CreationFromASearchDataByQueryShouldReturnAHsdpRequest()
    {
        var keyValuePair1 = KeyValuePair.Create("key1", "value1");
        var keyValuePair2 = KeyValuePair.Create("key2", "value2");
        var request = _hsdpRequestFactory.Create(new TdrSearchDataByQueryRequest(
            queryParameters: new List<KeyValuePair<string, string>>
            {
                keyValuePair1,
                keyValuePair2
            }),
            token: _token.Object
        );

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{TdrEndpoint}/store/tdr/DataItem"), request.Path);
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual(2, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "5"), request.Headers);
            Assert.AreEqual(2, request.QueryParameters.Count);
            Assert.Contains(keyValuePair1, request.QueryParameters);
            Assert.Contains(keyValuePair2, request.QueryParameters);
        });
    }

    [Test]
    public void CreationFromASearchDataByQueryWithRequestIdShouldReturnAHsdpRequestWithRequestIdHeader()
    {
        var request = _hsdpRequestFactory.Create(new TdrSearchDataByQueryRequest(
            queryParameters: new List<KeyValuePair<string, string>>()),
            token: _token.Object,
            requestId: "requestId"
        );

        Assert.Contains(KeyValuePair.Create("HSDP-Request-ID", "requestId"), request.Headers);
    }

    #endregion
}
