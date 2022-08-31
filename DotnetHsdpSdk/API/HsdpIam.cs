using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using DotnetHsdpSdk.Internal;

namespace DotnetHsdpSdk.API
{
    public class HsdpIam : IHsdpIam
    {
        private readonly HsdpIamConfiguration configuration;
        private IamToken? cachedServiceToken = null;

        public HsdpIam(HsdpIamConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest)
        {
            if (cachedServiceToken != null && !serviceLoginRequest.ForceRefetch)
            {
                return cachedServiceToken;
            }

            cachedServiceToken = await FetchServiceLoginToken(serviceLoginRequest);
            return cachedServiceToken;
        }

        private async Task<IamToken> FetchServiceLoginToken(IamServiceLoginRequest serviceLoginRequest)
        {
            var requestContent = CreateRequestContent(serviceLoginRequest);
            using var requestBody = new FormUrlEncodedContent(requestContent);
            using var request = new HttpRequestMessage(HttpMethod.Post, configuration.IamEndpoint);
            DecorateRequest(request, requestBody);

            var tokenResponse = await HttpGetTokenResponse(request);

            var accessToken = $"{tokenResponse.token_type} {tokenResponse.access_token}";
            return new IamToken(accessToken, DateTime.UtcNow.AddMinutes(tokenResponse.expires_in));
        }

        private static List<KeyValuePair<string, string>> CreateRequestContent(IamServiceLoginRequest serviceLoginRequest)
        {
            return new List<KeyValuePair<string, string>>
            {
                new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new("assertion", GenerateJwtToken(serviceLoginRequest))
            };
        }

        private static void DecorateRequest(HttpRequestMessage request, FormUrlEncodedContent requestBody)
        {
            request.Content = requestBody;
            request.Headers.Add("api-version", "2");
            request.Headers.Add("Accept", "application/json");
        }

        private static async Task<TokenResponse> HttpGetTokenResponse(HttpRequestMessage request)
        {
            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new AuthenticationException("Failed to acquire service token");
            }

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);
                if (tokenResponse == null) throw new AuthenticationException("Unable to parse TokenResponse from json.");
                return tokenResponse;
            }
            catch (JsonException e)
            {
                throw new AuthenticationException("Unable to parse TokenResponse from JSON.", e);
            }
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
