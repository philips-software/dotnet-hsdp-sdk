using System.Text;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.IAM.Internal;
using DotnetHsdpSdk.Utils;
using Moq;
using NUnit.Framework;
using DateTime = System.DateTime;

namespace DotnetHsdpSdkTests.IAM.Internal;

[TestFixture]
public class HsdpRequestFactoryTests
{
    private const string IamEndpoint = "https://endpoint.com";
    private const string ClientId = "clientId";
    private const string ClientSecret = "clientSecret";
    private Mock<IJwtSecurityTokenProvider> _securityTokenProvider = null!;
    private HsdpRequestFactory _hsdpRequestFactory = null!;
    private readonly string _basicAuth = Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}").EncodeBase64();

    private static readonly IamToken IamToken = new (
        "accessToken", 
        DateTime.UtcNow.AddHours(1),
        "Bearer",
        "scopes",
        "idToken",
        "signedToken",
        "issuedTokenType",
        "refreshToken"
    );

    [SetUp]
    public void SetUp()
    {
        _securityTokenProvider = new Mock<IJwtSecurityTokenProvider>();
        _hsdpRequestFactory = new HsdpRequestFactory(
            new HsdpIamConfiguration(IamEndpoint, ClientId, ClientSecret),
            _securityTokenProvider.Object);

        _securityTokenProvider.Setup(f => f.GenerateJwtToken("key", "aud", "id")).Returns("key.aud.id");
    }

    [Test]
    public async Task CreationFromIamUserLoginRequestShouldReturnAnHsdpRequestWithRequiredHeadersAndContent()
    {
        var request = _hsdpRequestFactory.CreateUserLoginRequest(new IamUserLoginRequest("username", "password"));
        
        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{IamEndpoint}/authorize/oauth2/token"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", $"Basic {_basicAuth}"), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "2"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", "application/json"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual("application/x-www-form-urlencoded", request.Content?.Headers?.ContentType?.MediaType);
            Assert.AreEqual("grant_type=password&username=username&password=password", content);
        });
    }

    [Test]
    public async Task CreationFromIamServiceLoginRequestShouldReturnAnHsdpRequestWithRequiredHeadersAndContent()
    {
        var request = _hsdpRequestFactory.CreateServiceLoginRequest(new IamServiceLoginRequest("key","aud","id"));
        
        var content = await request.Content!.ReadAsStringAsync();
        
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{IamEndpoint}/authorize/oauth2/token"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(2, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("api-version", "2"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", "application/json"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual("application/x-www-form-urlencoded", request.Content?.Headers?.ContentType?.MediaType);
            Assert.IsTrue(content.Contains("grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Ajwt-bearer&assertion=key.aud.id"));
        });
    }

    [Test]
    public async Task CreationFromIamRefreshRequestShouldReturnAnHsdpRequestWithRequiredHeadersAndContent()
    {
        var request = _hsdpRequestFactory.CreateRefreshRequest(IamToken);
        
        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{IamEndpoint}/authorize/oauth2/token"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", $"Basic {_basicAuth}"), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "2"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", "application/json"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual("application/x-www-form-urlencoded", request.Content?.Headers?.ContentType?.MediaType);
            Assert.AreEqual("grant_type=refresh_token&refresh_token=refreshToken", content);
        });
    }

    [Test]
    public async Task CreationFromIamRevokeRequestShouldReturnAnHsdpRequestWithRequiredHeadersAndContent()
    {
        var request = _hsdpRequestFactory.CreateRevokeRequest(IamToken);
        
        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{IamEndpoint}/authorize/oauth2/revoke"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", $"Basic {_basicAuth}"), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "2"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", "application/json"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual("application/x-www-form-urlencoded", request.Content?.Headers?.ContentType?.MediaType);
            Assert.AreEqual("token=accessToken", content);
        });
    }

    [Test]
    public async Task CreationFromIamIntrospectRequestShouldReturnAnHsdpRequestWithRequiredHeadersAndContent()
    {
        var request = _hsdpRequestFactory.CreateIntrospectRequest(IamToken);
        
        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{IamEndpoint}/authorize/oauth2/introspect"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", $"Basic {_basicAuth}"), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "4"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", "application/json"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual("application/x-www-form-urlencoded", request.Content?.Headers?.ContentType?.MediaType);
            Assert.AreEqual("token=accessToken", content);
        });
    }

    [Test]
    public async Task CreationFromIamUserInfoRequestShouldReturnAnHsdpRequestWithRequiredHeadersAndContent()
    {
        var request = _hsdpRequestFactory.CreateUserInfoRequest(IamToken);
        
        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri($"{IamEndpoint}/authorize/oauth2/userinfo"), request.Path);
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", $"Bearer {IamToken.AccessToken}"), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "2"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", "application/json"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
        });
    }

}
