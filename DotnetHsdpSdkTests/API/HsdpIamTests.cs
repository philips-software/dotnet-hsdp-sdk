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
        private const string Username = "TestUsername";
        private const string Password = "TestPassword";

        private readonly HsdpIamConfiguration configuration = new HsdpIamConfiguration(
            new Uri("http://localhost"),
            "testClientId",
            "testClientSecret"
        );

        private Mock<IHttp> http = null!;
        private HsdpIam iam = null!;

        [SetUp]
        public void SetUp()
        {
            http = new Mock<IHttp>();

            iam = new HsdpIam(configuration, http.Object);
        }

        [Test]
        public async Task UserLoginShouldReturnToken()
        {
            var requests = new List<HttpRequestMessage>();
            var response = new TokenResponse();
            http.Setup(h => h.HttpSendRequest<TokenResponse>(Capture.In(requests))).ReturnsAsync(response);

            var token = await iam.UserLogin(new IamUserLoginRequest(Username, Password));

        }
    }
}
