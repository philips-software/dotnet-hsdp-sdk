using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DotnetHsdpSdk.Internal;
using DotnetHsdpSdk.Utils;

[assembly: InternalsVisibleTo("DotnetHsdpSdkTests")]
namespace DotnetHsdpSdk.API
{
    public class HsdpIam : IHsdpIam
    {
        private readonly IHttpRequester http;
        private readonly IHsdpIamRequestFactory hsdpIamRequestFactory;
        private const string TokenPath = "authorize/oauth2/token";
        private const string RevokePath = "authorize/oauth2/revoke";
        private const string IntrospectPath = "authorize/oauth2/introspect";
        private const string UserInfoPath = "authorize/oauth2/userinfo";

        public HsdpIam(HsdpIamConfiguration configuration)
            : this(new HttpRequester(configuration, new Http()), new HsdpIamRequestFactory())
        {
        }

        internal HsdpIam(IHttpRequester http, IHsdpIamRequestFactory hsdpIamRequestFactory)
        {
            this.http = http;
            this.hsdpIamRequestFactory = hsdpIamRequestFactory;
        }

        public async Task<IIamToken> UserLogin(IamUserLoginRequest userLoginRequest)
        {
            var requestContent = hsdpIamRequestFactory.CreateUserLoginRequestContent(userLoginRequest);
            var tokenResponse = await http.HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath);
            return CreateIamToken(tokenResponse);
        }

        public async Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest)
        {
            var requestContent = hsdpIamRequestFactory.CreateServiceLoginRequestContent(serviceLoginRequest);
            var tokenResponse = await http.HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath);
            return CreateIamToken(tokenResponse);
        }

        public async Task<IIamToken> RefreshToken(IIamToken token)
        {
            ValidateToken(token);
            if (string.IsNullOrEmpty(token.RefreshToken)) throw new InvalidOperationException("Provided token cannot be refreshed. (RefreshToken is null or empty.)");

            var requestContent = hsdpIamRequestFactory.CreateRefreshRequestContent(token);
            var tokenResponse = await http.HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath);
            return CreateIamToken(tokenResponse);
        }

        public async Task RevokeToken(IIamToken token)
        {
            ValidateToken(token);

            var t = token as IamToken;
            if (t == null) throw new InvalidOperationException("Provided token is not of expected type.");

            var requestContent = hsdpIamRequestFactory.CreateRevokeRequestContent(token);
            await http.HttpRequestWithBasicAuth(requestContent, RevokePath);
            
            t.MarkAsRevoked();
        }

        public async Task<TokenMetadata> Introspect(IIamToken token)
        {
            ValidateToken(token);

            var requestContent = hsdpIamRequestFactory.CreateIntrospectRequestContent(token);
            return await http.HttpRequestWithBasicAuth<TokenMetadata>(requestContent, IntrospectPath);
        }

        public async Task<HsdpUserInfo> GetUserInfo(IIamToken token)
        {
            ValidateToken(token);
            var requestContent = hsdpIamRequestFactory.CreateEmptyRequestContent();
            return await http.HttpRequestWithBearerAuth<HsdpUserInfo>(requestContent, UserInfoPath, HttpMethod.Get, token);
        }

        private static IamToken CreateIamToken(TokenResponse tokenResponse)
        {
            return new IamToken(
                accessToken: tokenResponse.access_token,
                expiresUtc: DateTime.UtcNow.AddMinutes(tokenResponse.expires_in),
                tokenType: tokenResponse.token_type,
                scopes: tokenResponse.scopes,
                idToken: tokenResponse.id_token,
                signedToken: tokenResponse.signed_token,
                issuedTokenType: tokenResponse.issued_token_type,
                refreshToken: tokenResponse.refresh_token
            );
        }

        private static void ValidateToken(IIamToken token)
        {
            Validate.NotNull(token, nameof(token));
            if (!token.IsValid()) throw new InvalidOperationException("Provided token is not valid.");
        }
    }
}
