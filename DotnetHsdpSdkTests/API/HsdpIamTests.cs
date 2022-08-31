using DotnetHsdpSdk;
using DotnetHsdpSdk.API;
using DotnetHsdpSdk.Internal;
using DotnetHsdpSdk.Utils;
using Moq;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.API
{
    [TestFixture]
    public class HsdpIamTests
    {
        private readonly IamUserLoginRequest userLogin = new("testusername", "testpassword");
        private readonly IamServiceLoginRequest serviceLogin = new("testservicekey", "testserviceaudience", "testserviceid");

        private const string TokenPath = "authorize/oauth2/token";
        private const string RevokePath = "authorize/oauth2/revoke";
        private const string IntrospectPath = "authorize/oauth2/introspect";
        private const string UserInfoPath = "authorize/oauth2/userinfo";

        private readonly TokenResponse tokenResponse = new TokenResponse
        {
            access_token = "testaccesstoken",
            expires_in = 1234,
            id_token = "testidtoken",
            issued_token_type = "testissuedtokentype",
            refresh_token = "testrefreshtoken",
            scopes = "testscopes",
            signed_token = "testsignedtoken",
            token_type = "testtokentype"
        };

        private Mock<IHttpRequester> http = null!;
        private Mock<IHsdpIamRequestFactory> requestFactory = null!;
        private Mock<IHsdpIamRequest> request = null!;
        private HsdpIam iam = null!;

        [SetUp]
        public void SetUp()
        {
            http = new Mock<IHttpRequester>();
            requestFactory = new Mock<IHsdpIamRequestFactory>();
            request = new Mock<IHsdpIamRequest>();

            iam = new HsdpIam(http.Object, requestFactory.Object);
        }

        [Test]
        public async Task UserLoginShouldHttpRequestWithBasicAuth()
        {
            requestFactory.Setup(f => f.CreateUserLoginRequestContent(userLogin)).Returns(request.Object);
            http.Setup(h => h.HttpRequestWithBasicAuth<TokenResponse>(request.Object, TokenPath)).ReturnsAsync(tokenResponse);

            await iam.UserLogin(userLogin);

            http.Verify(h => h.HttpRequestWithBasicAuth<TokenResponse>(request.Object, TokenPath));
        }

        [Test]
        public async Task UserLoginShouldReturnToken()
        {
            requestFactory.Setup(f => f.CreateUserLoginRequestContent(userLogin)).Returns(request.Object);
            http.Setup(h => h.HttpRequestWithBasicAuth<TokenResponse>(request.Object, TokenPath)).ReturnsAsync(tokenResponse);

            var token = await iam.UserLogin(userLogin);

            AssertToken(token);
        }

        [Test]
        public async Task ServiceLoginShouldHttpRequestWithBasicAuth()
        {
            requestFactory.Setup(f => f.CreateServiceLoginRequestContent(serviceLogin)).Returns(request.Object);
            http.Setup(h => h.HttpRequestWithBasicAuth<TokenResponse>(request.Object, TokenPath)).ReturnsAsync(tokenResponse);

            await iam.ServiceLogin(serviceLogin);

            http.Verify(h => h.HttpRequestWithBasicAuth<TokenResponse>(request.Object, TokenPath));
        }

        [Test]
        public async Task ServiceLoginShouldReturnToken()
        {
            requestFactory.Setup(f => f.CreateServiceLoginRequestContent(serviceLogin)).Returns(request.Object);
            http.Setup(h => h.HttpRequestWithBasicAuth<TokenResponse>(request.Object, TokenPath)).ReturnsAsync(tokenResponse);

            var token = await iam.ServiceLogin(serviceLogin);

            AssertToken(token);
        }

        private void AssertToken(IIamToken token)
        {
            Assert.That(token.ExpiresUtc, Is.EqualTo(DateTime.UtcNow + TimeSpan.FromMinutes(tokenResponse.expires_in)).Within(TimeSpan.FromMinutes(1)));
            Assert.That(token.AccessToken, Is.EqualTo(tokenResponse.token_type + " " + tokenResponse.access_token));
            Assert.That(token.IdToken, Is.EqualTo(tokenResponse.id_token));
            Assert.That(token.IssuedTokenType, Is.EqualTo(tokenResponse.issued_token_type));
            Assert.That(token.TokenType, Is.EqualTo(tokenResponse.token_type));
            Assert.That(token.RefreshToken, Is.EqualTo(tokenResponse.refresh_token));
            Assert.That(token.Scopes, Is.EqualTo(tokenResponse.scopes));
            Assert.That(token.SignedToken, Is.EqualTo(tokenResponse.signed_token));
        }
    }
}
