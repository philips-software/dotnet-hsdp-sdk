using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DotnetHsdpSdk.Utils;
using Microsoft.IdentityModel.Tokens;

namespace DotnetHsdpSdk.IAM.Internal;

public interface IRequestFactory
{
    IHsdpRequest CreateServiceLoginRequest(IamServiceLoginRequest serviceLoginRequest);
    IHsdpRequest CreateUserLoginRequest(IamUserLoginRequest userLoginRequest);
    IHsdpRequest CreateRefreshRequest(IIamToken token);
    IHsdpRequest CreateRevokeRequest(IIamToken token);
    IHsdpRequest CreateIntrospectRequest(IIamToken token);
    IHsdpRequest CreateUserInfoRequest(IIamToken token);
}

internal class RequestFactory : IRequestFactory
{
    private const string TokenPath = "authorize/oauth2/token";
    private const string RevokePath = "authorize/oauth2/revoke";
    private const string IntrospectPath = "authorize/oauth2/introspect";
    private const string UserInfoPath = "authorize/oauth2/userinfo";

    private readonly Uri _iamEndpoint;
    private readonly string _basicAuthentication;

    public RequestFactory(HsdpIamConfiguration configuration)
    {
        _iamEndpoint = configuration.IamEndpoint;
        _basicAuthentication = configuration.BasicAuthentication;
    }

    public IHsdpRequest CreateServiceLoginRequest(IamServiceLoginRequest serviceLoginRequest)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri(_iamEndpoint, TokenPath),
            HeadersForNoAuth("2"),
            new List<KeyValuePair<string, string>> {
                new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new("assertion", GenerateJwtToken(serviceLoginRequest))
            }
        );
    }

    public IHsdpRequest CreateUserLoginRequest(IamUserLoginRequest userLoginRequest)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri(_iamEndpoint, TokenPath),
            HeadersForBasicAuth("2"),
            new List<KeyValuePair<string, string>>()
            {
                new("grant_type", "password"),
                new("username", userLoginRequest.Username),
                new("password", userLoginRequest.Password)
            }
        );
    }

    public IHsdpRequest CreateRefreshRequest(IIamToken token)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri(_iamEndpoint, TokenPath),
            HeadersForBasicAuth("2"),
            new List<KeyValuePair<string, string>> {
                new("grant_type", "refresh_token"),
                new("refresh_token", token.RefreshToken!)
            }
        );
    }

    public IHsdpRequest CreateRevokeRequest(IIamToken token)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri(_iamEndpoint, RevokePath),
            HeadersForBasicAuth("2"),
            new List<KeyValuePair<string, string>>
            {
                new("token", token.AccessToken)
            }
        );
    }

    public IHsdpRequest CreateIntrospectRequest(IIamToken token)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri(_iamEndpoint, IntrospectPath),
            HeadersForBasicAuth("4"),
            new List<KeyValuePair<string, string>>
            {
                new("token", token.AccessToken)                
            }
        );
    }

    public IHsdpRequest CreateUserInfoRequest(IIamToken token)
    {
        return CreateRequest(
            HttpMethod.Get,
            new Uri(_iamEndpoint, UserInfoPath),
            HeadersForBearerAuth("2", token.AccessToken),
            new List<KeyValuePair<string, string>>()
        );
    }

    private static IHsdpRequest CreateRequest(
        HttpMethod method,
        Uri path,
        IEnumerable<KeyValuePair<string, string>> headers,
        IEnumerable<KeyValuePair<string, string>> urlFormParameters
    )
    {
        var hsdpRequest = new HsdpRequest(method, path);
        foreach (var (key, value) in headers)
        {
            hsdpRequest.Headers.Add(new KeyValuePair<string, string>(key, value));
        }
        hsdpRequest.Content = new FormUrlEncodedContent(urlFormParameters);

        return hsdpRequest;
    }

    private static List<KeyValuePair<string, string>> HeadersForNoAuth(string version)
    {
        return new List<KeyValuePair<string, string>>
        {
            new("api-version", version),
            new("Accept", "application/json")
        };
    }

    private List<KeyValuePair<string, string>> HeadersForBasicAuth(string version)
    {
        return new List<KeyValuePair<string, string>>
        {
            new("api-version", version),
            new("Authorization", $"Basic {_basicAuthentication}"),
            new("Accept", "application/json")
        };
    }

    private static List<KeyValuePair<string, string>> HeadersForBearerAuth(string version, string accessToken)
    {
        return new List<KeyValuePair<string, string>>
        {
            new("api-version", version),
            new("Authorization", $"Bearer {accessToken}"),
            new("Accept", "application/json")
        };
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
