using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.IAM.Internal;
using DotnetHsdpSdk.Utils;
using Moq;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.IAM.Internal;

[TestFixture]
public class IamResponseFactoryTests
{
    private Mock<IDateTimeProvider> _dateTimeProvider = null!;
    private IIamResponseFactory _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _dateTimeProvider = new Mock<IDateTimeProvider>();
        _factory = new IamResponseFactory(_dateTimeProvider.Object);
        _dateTimeProvider.Setup(f => f.UtcNow).Returns(DateTime.Parse("2022-09-16T12:13:14.000Z"));
    }

    #region IamToken

    [Test]
    public void CreateIamTokenForSuccessfulCallShouldReturnAnIamToken()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenResponse)
        );
        
        var response = _factory.CreateIamToken(hsdpResponse);
        
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<IIamToken>(response);
            Assert.AreEqual(TestData.TokenResponse.access_token, response.AccessToken);
            var expectedExpiryTime = _dateTimeProvider.Object.UtcNow.AddMinutes(TestData.TokenResponse.expires_in);
            Assert.AreEqual(expectedExpiryTime, response.ExpiresUtc);
            Assert.AreEqual(TestData.TokenResponse.token_type, response.TokenType);
            Assert.AreEqual(TestData.TokenResponse.scopes, response.Scopes);
            Assert.AreEqual(TestData.TokenResponse.id_token, response.IdToken);
            Assert.AreEqual(TestData.TokenResponse.signed_token, response.SignedToken);
            Assert.AreEqual(TestData.TokenResponse.issued_token_type, response.IssuedTokenType);
            Assert.AreEqual(TestData.TokenResponse.refresh_token, response.RefreshToken);
        });
    }

    [Test]
    public void CreateIamTokenForFailingCallShouldThrowAnAuthenticationException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<AuthenticationException>(() =>
        {
            _factory.CreateIamToken(hsdpResponse);
        });
    }

    [Test]
    public void CreateIamTokenForANullBodyShouldThrowAnHsdpRequestException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateIamToken(hsdpResponse);
        });
    }

    [Test]
    public void CreateIamTokenForANonConformingBodyShouldThrowAnHsdpRequestException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"""{key"":""value}"""
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateIamToken(hsdpResponse);
        });
    }

    [Test]
    public void CreateIamTokenForAnInvalidJsonBodyShouldThrowAnHsdpRequestException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"""{invalid json syntax}"""
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateIamToken(hsdpResponse);
        });
    }
    
    #endregion
    
    #region IamTokenMetadata

    [Test]
    public void CreateIamTokenMetadataForSuccessfulIntrospectCallShouldReturnAnIamTokenMetadata()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.IntrospectResponse)
        );
        
        var response = _factory.CreateIamTokenMetadata(hsdpResponse);
        
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<IamTokenMetadata>(response);
            Assert.AreEqual(TestData.IntrospectResponse.active, response.IsActive);
            Assert.AreEqual(TestData.IntrospectResponse.scope, response.Scopes);
            Assert.AreEqual(TestData.IntrospectResponse.client_id, response.ClientId);
            Assert.AreEqual(TestData.IntrospectResponse.username, response.UserName);
            Assert.AreEqual(TestData.IntrospectResponse.token_type, response.TokenType);
            Assert.AreEqual(TestData.IntrospectResponse.exp, response.ExpirationTimeInEpochSeconds);
            Assert.AreEqual(TestData.IntrospectResponse.sub, response.Subject);
            Assert.AreEqual(TestData.IntrospectResponse.iss, response.Issuer);
            Assert.AreEqual(TestData.IntrospectResponse.identity_type, response.IdentityType);
            Assert.AreEqual(TestData.IntrospectResponse.device_type, response.DeviceType);
            Assert.IsInstanceOf<Organizations>(response.Organizations);
            Assert.AreEqual(TestData.IntrospectResponse.token_type_hint, response.TokenTypeHint);
            Assert.AreEqual(TestData.IntrospectResponse.client_organization_id, response.ClientOrganizationId);
            Assert.AreEqual(TestData.IntrospectResponse.act?.sub, response.Actor?.Sub);
        });
    }

    [Test]
    public void CreateIamTokenMetadataForFailingIntrospectCallShouldThrowAnAuthenticationException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<AuthenticationException>(() =>
        {
            _factory.CreateIamTokenMetadata(hsdpResponse);
        });
    }

    [Test]
    public void CreateIamTokenMetadataForANullBodyShouldThrowAnHsdpRequestException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateIamTokenMetadata(hsdpResponse);
        });
    }

    [Test]
    public void CreateIamTokenMetadataForANonConformingBodyShouldThrowAnHsdpRequestException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"""{key"":""value}"""
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateIamTokenMetadata(hsdpResponse);
        });
    }

    [Test]
    public void CreateIamTokenMetadataForAnInvalidJsonBodyShouldThrowAnHsdpRequestException()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"""{invalid json syntax}"""
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateIamTokenMetadata(hsdpResponse);
        });
    }
    
    #endregion

}
