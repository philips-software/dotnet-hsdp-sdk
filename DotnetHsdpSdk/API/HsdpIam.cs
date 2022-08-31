using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using DotnetHsdpSdk.Internal;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.API
{
    public class HsdpIam : IHsdpIam
    {
        private readonly HsdpIamConfiguration configuration;

        private const string TokenPath = "authorize/oauth2/token";
        private const string RevokePath = "authorize/oauth2/revoke";
        private const string IntrospectPath = "authorize/oauth2/introspect";
        private const string UserInfoPath = "authorize/oauth2/userinfo";

        public HsdpIam(HsdpIamConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IIamToken> UserLogin(IamUserLoginRequest userLoginRequest)
        {
            var requestContent = CreateUserLoginRequestContent(userLoginRequest);
            var tokenResponse = await HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath);
            return CreateIamToken(tokenResponse);
        }

        public async Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest)
        {
            var requestContent = CreateServiceLoginRequestContent(serviceLoginRequest);
            var tokenResponse = await HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath);
            return CreateIamToken(tokenResponse);
        }

        public async Task<IIamToken> RefreshToken(IIamToken token)
        {
            ValidateToken(token);
            if (string.IsNullOrEmpty(token.RefreshToken)) throw new InvalidOperationException("Provided token cannot be refreshed. (RefreshToken is null or empty.)");

            var requestContent = CreateRefreshRequestContent(token);
            var tokenResponse = await HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath);
            return CreateIamToken(tokenResponse);
        }

        public async Task RevokeToken(IIamToken token)
        {
            ValidateToken(token);

            var t = token as IamToken;
            if (t == null) throw new InvalidOperationException("Provided token is not of expected type.");

            var requestContent = CreateRevokeRequestContent(token);
            await HttpRequestWithBasicAuth(requestContent, RevokePath);
            
            t.MarkAsRevoked();
        }

        public async Task<TokenMetadata> Introspect(IIamToken token)
        {
            ValidateToken(token);

            var requestContent = CreateIntrospectRequestContent(token);
            return await HttpRequestWithBasicAuth<TokenMetadata>(requestContent, IntrospectPath);
        }

        public async Task<HsdpUserInfo> GetUserInfo(IIamToken token)
        {
            ValidateToken(token);

            using var requestBody = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, UserInfoPath));
            DecorateWithBearerAuth(request, requestBody, token);

            return await Http.HttpSendRequest<HsdpUserInfo>(request);
        }

        private async Task<T> HttpRequestWithBasicAuth<T>(List<KeyValuePair<string, string>> requestContent, string path)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateWithBasicAuth(request, requestBody);

            return await Http.HttpSendRequest<T>(request);
        }

        private async Task HttpRequestWithBasicAuth(List<KeyValuePair<string, string>> requestContent, string path)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateWithBasicAuth(request, requestBody);

            await Http.HttpSendRequest(request);
        }

        private static List<KeyValuePair<string, string>> CreateServiceLoginRequestContent(IamServiceLoginRequest serviceLoginRequest)
        {
            return new List<KeyValuePair<string, string>>
            {
                new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new("assertion", GenerateJwtToken(serviceLoginRequest))
            };
        }

        private static List<KeyValuePair<string, string>> CreateUserLoginRequestContent(IamUserLoginRequest userLoginRequest)
        {
            return new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("username", userLoginRequest.Username),
                new("password", userLoginRequest.Password)
            };
        }

        private static List<KeyValuePair<string, string>> CreateRefreshRequestContent(IIamToken token)
        {
            return new List<KeyValuePair<string, string>>
            {
                new("grant_type", "refresh_token"),
                new("refresh_token", token.RefreshToken!),
            };
        }

        private static List<KeyValuePair<string, string>> CreateRevokeRequestContent(IIamToken token)
        {
            return new List<KeyValuePair<string, string>>
            {
                new("token", token.AccessToken)
            };
        }

        private static List<KeyValuePair<string, string>> CreateIntrospectRequestContent(IIamToken token)
        {
            return CreateRevokeRequestContent(token);
        }

        private static IamToken CreateIamToken(TokenResponse tokenResponse)
        {
            var accessToken = $"{tokenResponse.token_type} {tokenResponse.access_token}";
            return new IamToken(
                accessToken: accessToken,
                expiresUtc: DateTime.UtcNow.AddMinutes(tokenResponse.expires_in),
                tokenType: tokenResponse.token_type,
                scopes: tokenResponse.scopes,
                idToken: tokenResponse.id_token,
                signedToken: tokenResponse.signed_token,
                issuedTokenType: tokenResponse.issued_token_type,
                refreshToken: tokenResponse.refresh_token
            );
        }

        private void DecorateWithBasicAuth(HttpRequestMessage request, FormUrlEncodedContent requestBody)
        {
            DecorateRequest(request, requestBody);
            request.Headers.Add("Authorization", $"Basic {configuration.BasicAuthentication}");
        }

        private static void DecorateWithBearerAuth(HttpRequestMessage request, FormUrlEncodedContent requestBody, IIamToken token)
        {
            DecorateRequest(request, requestBody);
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
        }

        private static void DecorateRequest(HttpRequestMessage request, FormUrlEncodedContent requestBody)
        {
            request.Content = requestBody;
            request.Headers.Add("api-version", "2");
            request.Headers.Add("Accept", "application/json");
        }

        private static void ValidateToken(IIamToken token)
        {
            Validate.NotNull(token, nameof(token));
            if (!token.IsValid()) throw new InvalidOperationException("Provided token is not valid.");
        }

        private static string GenerateJwtToken(IamServiceLoginRequest serviceLoginRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var privateKeyBase64 = Regex.Replace(serviceLoginRequest.ServiceKey, "-----BEGIN RSA PRIVATE KEY-----\n?", "");
            privateKeyBase64 = Regex.Replace(privateKeyBase64, "\n?-----END RSA PRIVATE KEY-----\n?", "");
            try
            {
                var privateKey = Convert.FromBase64String(privateKeyBase64);
                using RSA rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(privateKey, out _);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = serviceLoginRequest.ServiceAudience,
                    Issuer = serviceLoginRequest.ServiceId,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
                    {
                        CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
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
}
