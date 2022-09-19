using System;
using System.Collections.Generic;
using System.Net.Http;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.IAM.Internal;

public interface IHsdpRequestFactory
{
    IHsdpRequest CreateServiceLoginRequest(IamServiceLoginRequest serviceLoginRequest);
    IHsdpRequest CreateUserLoginRequest(IamUserLoginRequest userLoginRequest);
    IHsdpRequest CreateRefreshRequest(IIamToken token);
    IHsdpRequest CreateRevokeRequest(IIamToken token);
    IHsdpRequest CreateIntrospectRequest(IIamToken token);
    IHsdpRequest CreateUserInfoRequest(IIamToken token);
}

internal class HsdpRequestFactory : IHsdpRequestFactory
{
    private const string TokenPath = "authorize/oauth2/token";
    private const string RevokePath = "authorize/oauth2/revoke";
    private const string IntrospectPath = "authorize/oauth2/introspect";
    private const string UserInfoPath = "authorize/oauth2/userinfo";
    private readonly string _basicAuthentication;

    private readonly string _iamEndpoint;
    private readonly IJwtSecurityTokenProvider _securityTokenProvider;

    public HsdpRequestFactory(HsdpIamConfiguration configuration, IJwtSecurityTokenProvider securityTokenProvider)
    {
        _iamEndpoint = configuration.IamEndpoint;
        _basicAuthentication = configuration.BasicAuthentication;
        _securityTokenProvider = securityTokenProvider;
    }

    public IHsdpRequest CreateServiceLoginRequest(IamServiceLoginRequest serviceLoginRequest)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri($"{_iamEndpoint}/{TokenPath}"),
            HeadersForNoAuth("2"),
            new List<KeyValuePair<string, string>>
            {
                new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new("assertion", _securityTokenProvider.GenerateJwtToken(
                    serviceLoginRequest.ServiceKey, serviceLoginRequest.ServiceAudience, serviceLoginRequest.ServiceId
                ))
            }
        );
    }

    public IHsdpRequest CreateUserLoginRequest(IamUserLoginRequest userLoginRequest)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri($"{_iamEndpoint}/{TokenPath}"),
            HeadersForBasicAuth("2"),
            new List<KeyValuePair<string, string>>
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
            new Uri($"{_iamEndpoint}/{TokenPath}"),
            HeadersForBasicAuth("2"),
            new List<KeyValuePair<string, string>>
            {
                new("grant_type", "refresh_token"),
                new("refresh_token", token.RefreshToken!)
            }
        );
    }

    public IHsdpRequest CreateRevokeRequest(IIamToken token)
    {
        return CreateRequest(
            HttpMethod.Post,
            new Uri($"{_iamEndpoint}/{RevokePath}"),
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
            new Uri($"{_iamEndpoint}/{IntrospectPath}"),
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
            new Uri($"{_iamEndpoint}/{UserInfoPath}"),
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
        foreach (var (key, value) in headers) hsdpRequest.Headers.Add(KeyValuePair.Create(key, value));
        hsdpRequest.Content = new FormUrlEncodedContent(urlFormParameters);

        return hsdpRequest;
    }

    private static IEnumerable<KeyValuePair<string, string>> HeadersForNoAuth(string version)
    {
        return new List<KeyValuePair<string, string>>
        {
            new("api-version", version),
            new("Accept", "application/json")
        };
    }

    private IEnumerable<KeyValuePair<string, string>> HeadersForBasicAuth(string version)
    {
        return new List<KeyValuePair<string, string>>
        {
            new("api-version", version),
            new("Authorization", $"Basic {_basicAuthentication}"),
            new("Accept", "application/json")
        };
    }

    private static IEnumerable<KeyValuePair<string, string>> HeadersForBearerAuth(string version, string accessToken)
    {
        return new List<KeyValuePair<string, string>>
        {
            new("api-version", version),
            new("Authorization", $"Bearer {accessToken}"),
            new("Accept", "application/json")
        };
    }
}
