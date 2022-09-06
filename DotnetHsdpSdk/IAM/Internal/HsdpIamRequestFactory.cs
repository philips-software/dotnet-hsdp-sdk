using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace DotnetHsdpSdk.IAM.Internal;

public interface IHsdpIamRequestFactory
{
    IHsdpIamRequest CreateServiceLoginRequestContent(IamServiceLoginRequest serviceLoginRequest);
    IHsdpIamRequest CreateUserLoginRequestContent(IamUserLoginRequest userLoginRequest);
    IHsdpIamRequest CreateRefreshRequestContent(IIamToken token);
    IHsdpIamRequest CreateRevokeRequestContent(IIamToken token);
    IHsdpIamRequest CreateIntrospectRequestContent(IIamToken token);
    IHsdpIamRequest CreateEmptyRequestContent();
}

public interface IHsdpIamRequest
{
    List<KeyValuePair<string, string>> Content { get; }
}

internal class HsdpIamRequestFactory : IHsdpIamRequestFactory
{
    public IHsdpIamRequest CreateServiceLoginRequestContent(IamServiceLoginRequest serviceLoginRequest)
    {
        return CreateRequest(
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
            new KeyValuePair<string, string>("assertion", GenerateJwtToken(serviceLoginRequest))
        );
    }

    public IHsdpIamRequest CreateUserLoginRequestContent(IamUserLoginRequest userLoginRequest)
    {
        return CreateRequest(
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", userLoginRequest.Username),
            new KeyValuePair<string, string>("password", userLoginRequest.Password)
        );
    }

    public IHsdpIamRequest CreateRefreshRequestContent(IIamToken token)
    {
        return CreateRequest(
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", token.RefreshToken!)
        );
    }

    public IHsdpIamRequest CreateRevokeRequestContent(IIamToken token)
    {
        return CreateRequest(
            new KeyValuePair<string, string>("token", token.AccessToken)
        );
    }

    public IHsdpIamRequest CreateIntrospectRequestContent(IIamToken token)
    {
        return CreateRequest(
            new KeyValuePair<string, string>("token", token.AccessToken)
        );
    }

    public IHsdpIamRequest CreateEmptyRequestContent()
    {
        return CreateRequest();
    }

    private static HsdpIamRequest CreateRequest(params KeyValuePair<string, string>[] elements)
    {
        return new HsdpIamRequest(elements.ToList());
    }

    private static string GenerateJwtToken(IamServiceLoginRequest serviceLoginRequest)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var privateKeyBase64 = Regex.Replace(serviceLoginRequest.ServiceKey, "\n", "");
        privateKeyBase64 = Regex.Replace(privateKeyBase64, "-----BEGIN RSA PRIVATE KEY-----", "");
        privateKeyBase64 = Regex.Replace(privateKeyBase64, "-----END RSA PRIVATE KEY-----", "");
        try
        {
            var privateKey = Convert.FromBase64String(privateKeyBase64);
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = serviceLoginRequest.ServiceAudience,
                Issuer = serviceLoginRequest.ServiceId,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory {CacheSignatureProviders = false}
                }
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Payload.Add("sub", serviceLoginRequest.ServiceId);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception e)
        {
            throw new AuthenticationException("Failed to create service token: " + e);
        }
    }
}

internal class HsdpIamRequest : IHsdpIamRequest
{
    public HsdpIamRequest(List<KeyValuePair<string, string>> content)
    {
        Content = content;
    }

    public List<KeyValuePair<string, string>> Content { get; }
}