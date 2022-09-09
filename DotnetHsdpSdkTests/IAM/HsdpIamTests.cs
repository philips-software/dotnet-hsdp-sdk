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

    private readonly HsdpResponse<TokenResponse> _tokenResponse = new(
        new List<KeyValuePair<string, string>>(),
        new TokenResponse
        {
            access_token = "testaccesstoken",
            expires_in = 1234,
            id_token = "testidtoken",
            issued_token_type = "testissuedtokentype",
            refresh_token = "testrefreshtoken",
            scopes = "testscopes",
            signed_token = "testsignedtoken",
            token_type = "testtokentype"
        }
    );

    private Mock<IHttpRequester> _http = null!;
    private Mock<IRequestFactory> _requestFactory = null!;
    private Mock<IHsdpRequest> _request = null!;
    private HsdpIam _iam = null!;

    [SetUp]
    public void SetUp()
    {
        _http = new Mock<IHttpRequester>();
        _requestFactory = new Mock<IRequestFactory>();
        _request = new Mock<IHsdpRequest>();

        _iam = new HsdpIam(_http.Object, _requestFactory.Object);
    }

    [Test]
    public async Task UserLoginShouldHttpRequestWithBasicAuth()
    {
        _requestFactory.Setup(f => f.CreateUserLoginRequest(userLogin)).Returns(_request.Object);
        _http.Setup(h => h.HttpRequest<TokenResponse>(_request.Object)).ReturnsAsync(_tokenResponse);

        await _iam.UserLogin(userLogin);

        _http.Verify(h => h.HttpRequest<TokenResponse>(_request.Object));
    }

    [Test]
    public async Task UserLoginShouldReturnToken()
    {
        _requestFactory.Setup(f => f.CreateUserLoginRequest(userLogin)).Returns(_request.Object);
        _http.Setup(h => h.HttpRequest<TokenResponse>(_request.Object)).ReturnsAsync(_tokenResponse);

        var token = await _iam.UserLogin(userLogin);

        AssertToken(token);
    }

    [Test]
    public async Task ServiceLoginShouldHttpRequestWithBasicAuth()
    {
        _requestFactory.Setup(f => f.CreateServiceLoginRequest(serviceLogin)).Returns(_request.Object);
        _http.Setup(h => h.HttpRequest<TokenResponse>(_request.Object)).ReturnsAsync(_tokenResponse);

        await _iam.ServiceLogin(serviceLogin);

        _http.Verify(h => h.HttpRequest<TokenResponse>(_request.Object));
    }

    [Test]
    public async Task ServiceLoginShouldReturnToken()
    {
        _requestFactory.Setup(f => f.CreateServiceLoginRequest(serviceLogin)).Returns(_request.Object);
        _http.Setup(h => h.HttpRequest<TokenResponse>(_request.Object)).ReturnsAsync(_tokenResponse);

        var token = await _iam.ServiceLogin(serviceLogin);

        AssertToken(token);
    }

    private void AssertToken(IIamToken token)
    {
        Assert.That(token.ExpiresUtc, Is.EqualTo(DateTime.UtcNow + TimeSpan.FromMinutes(_tokenResponse.Body.expires_in)).Within(TimeSpan.FromMinutes(1)));
        Assert.That(token.AccessToken, Is.EqualTo(_tokenResponse.Body.access_token));
        Assert.That(token.IdToken, Is.EqualTo(_tokenResponse.Body.id_token));
        Assert.That(token.IssuedTokenType, Is.EqualTo(_tokenResponse.Body.issued_token_type));
        Assert.That(token.TokenType, Is.EqualTo(_tokenResponse.Body.token_type));
        Assert.That(token.RefreshToken, Is.EqualTo(_tokenResponse.Body.refresh_token));
        Assert.That(token.Scopes, Is.EqualTo(_tokenResponse.Body.scopes));
        Assert.That(token.SignedToken, Is.EqualTo(_tokenResponse.Body.signed_token));
    }
}
