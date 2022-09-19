using System.Net;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.IAM.Internal;
using DotnetHsdpSdk.Utils;
using Moq;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.IAM;

[TestFixture]
public class HsdpIamTests
{
    private readonly IamUserLoginRequest userLogin = new("testusername", "testpassword");
    private readonly IamServiceLoginRequest serviceLogin = new("testservicekey", "testserviceaudience", "testserviceid");

    private Mock<IHttpRequester> _http = null!;
    private Mock<IHsdpRequestFactory> _requestFactory = null!;
    private Mock<IIamResponseFactory> _responseFactory = null!;
    private Mock<IHsdpRequest> _request = null!;
    private Mock<IHsdpResponse<string>> _response = null!;
    private HsdpIam _iam = null!;

    [SetUp]
    public void SetUp()
    {
        _http = new Mock<IHttpRequester>();
        _requestFactory = new Mock<IHsdpRequestFactory>();
        _responseFactory = new Mock<IIamResponseFactory>();
        _request = new Mock<IHsdpRequest>();
        _response = new Mock<IHsdpResponse<string>>();

        _iam = new HsdpIam(_http.Object, _requestFactory.Object, _responseFactory.Object);
    }

    [Test]
    public async Task UserLoginShouldHttpRequestWithBasicAuth()
    {
        var token = new Mock<IIamToken>();
        _requestFactory.Setup(f => f.CreateUserLoginRequest(userLogin)).Returns(_request.Object);
        _http.Setup(h => h.HttpRequest<string>(_request.Object)).ReturnsAsync(_response.Object);
        _responseFactory.Setup(f => f.CreateIamToken(_response.Object)).Returns(token.Object);

        var result =await _iam.UserLogin(userLogin);

        _requestFactory.Verify(h => h.CreateUserLoginRequest(userLogin));
        _http.Verify(h => h.HttpRequest<string>(_request.Object));
        _responseFactory.Verify(h => h.CreateIamToken(_response.Object));
        Assert.AreEqual(token.Object, result);
    }

    [Test]
    public async Task ServiceLoginShouldHttpRequestWithBasicAuth()
    {
        var token = new Mock<IIamToken>();
        _requestFactory.Setup(f => f.CreateServiceLoginRequest(serviceLogin)).Returns(_request.Object);
        _http.Setup(h => h.HttpRequest<string>(_request.Object)).ReturnsAsync(_response.Object);
        _responseFactory.Setup(f => f.CreateIamToken(_response.Object)).Returns(token.Object);

        var result = await _iam.ServiceLogin(serviceLogin);

        _requestFactory.Verify(h => h.CreateServiceLoginRequest(serviceLogin));
        _http.Verify(h => h.HttpRequest<string>(_request.Object));
        _responseFactory.Verify(h => h.CreateIamToken(_response.Object));
        Assert.AreEqual(token.Object, result);
    }
}
