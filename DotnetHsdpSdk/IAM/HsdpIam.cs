using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DotnetHsdpSdk.IAM.Internal;
using DotnetHsdpSdk.Utils;

[assembly: InternalsVisibleTo("DotnetHsdpSdkTests")]

namespace DotnetHsdpSdk.IAM;

public class HsdpIam : IHsdpIam
{
    private readonly IHsdpRequestFactory _hsdpRequestFactory;
    private readonly IHttpRequester _http;
    private readonly IIamResponseFactory _iamResponseFactory;

    public HsdpIam(
        HsdpIamConfiguration configuration,
        IDateTimeProvider dateTimeProvider,
        IJwtSecurityTokenProvider securityTokenProvider
    ) : this(
        new HttpRequester(),
        new HsdpRequestFactory(configuration, securityTokenProvider),
        new IamResponseFactory(dateTimeProvider)
    )
    {
    }

    internal HsdpIam(IHttpRequester http, IHsdpRequestFactory hsdpRequestFactory,
        IIamResponseFactory iamResponseFactory)
    {
        _http = http;
        _hsdpRequestFactory = hsdpRequestFactory;
        _iamResponseFactory = iamResponseFactory;
    }

    public async Task<IIamToken> UserLogin(IamUserLoginRequest userLoginRequest)
    {
        var request = _hsdpRequestFactory.CreateUserLoginRequest(userLoginRequest);
        var response = await _http.HttpRequest<string>(request);
        return _iamResponseFactory.CreateIamToken(response);
    }

    public async Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest)
    {
        var request = _hsdpRequestFactory.CreateServiceLoginRequest(serviceLoginRequest);
        var response = await _http.HttpRequest<string>(request);
        return _iamResponseFactory.CreateIamToken(response);
    }

    public async Task<IIamToken> RefreshToken(IIamToken token)
    {
        ValidateToken(token);
        if (string.IsNullOrEmpty(token.RefreshToken))
            throw new InvalidOperationException("Provided token cannot be refreshed. (RefreshToken is null or empty.)");

        var request = _hsdpRequestFactory.CreateRefreshRequest(token);
        var response = await _http.HttpRequest<string>(request);
        return _iamResponseFactory.CreateIamToken(response);
    }

    public async Task RevokeToken(IIamToken token)
    {
        ValidateToken(token);
        var t = token as IamToken;
        if (t == null) throw new InvalidOperationException("Provided token is not of expected type.");
        var request = _hsdpRequestFactory.CreateRevokeRequest(token);
        await _http.HttpRequest(request);
        t.MarkAsRevoked();
    }

    public async Task<IamTokenMetadata> Introspect(IIamToken token)
    {
        ValidateToken(token);
        var request = _hsdpRequestFactory.CreateIntrospectRequest(token);
        var response = await _http.HttpRequest<string>(request);
        return _iamResponseFactory.CreateIamTokenMetadata(response);
    }

    public async Task<IamUserInfo> GetUserInfo(IIamToken token)
    {
        ValidateToken(token);
        var request = _hsdpRequestFactory.CreateUserInfoRequest(token);
        var response = await _http.HttpRequest<string>(request);
        return _iamResponseFactory.CreateIamUserInfo(response);
    }

    private static void ValidateToken(IIamToken token)
    {
        Validate.NotNull(token, nameof(token));
        if (!token.IsValid()) throw new InvalidOperationException("Provided token is not valid.");
    }
}
