using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DotnetHsdpSdkTests")]
namespace DotnetHsdpSdk.Internal
{
    internal class IamToken : IIamToken
    {
        public IamToken(string accessToken, DateTime expiresUtc, string tokenType, string scopes, string idToken, string signedToken, string issuedTokenType, string? refreshToken = null)
        {
            AccessToken = accessToken;
            ExpiresUtc = expiresUtc;
            TokenType = tokenType;
            Scopes = scopes;
            IdToken = idToken;
            SignedToken = signedToken;
            IssuedTokenType = issuedTokenType;
            RefreshToken = refreshToken;
        }

        public string AccessToken { get; private set; }
        public DateTime ExpiresUtc { get; }
        public string TokenType { get; }
        public string Scopes { get; }
        public string IdToken { get; }
        public string SignedToken { get; }
        public string IssuedTokenType { get; }
        public string? RefreshToken { get; }

        public bool IsExpired()
        {
            return ExpiresUtc < DateTime.UtcNow;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(TokenType) && !IsExpired();
        }

        public void MarkAsRevoked()
        {
            AccessToken = string.Empty;
        }
    }

    internal class TokenResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        
        public string access_token { get; set; } = "";
        public int expires_in { get; set; }
        public string token_type { get; set; } = "";
        public string refresh_token { get; set; } = "";
        public string scopes { get; set; } = "";
        public string id_token { get; set; } = "";
        public string signed_token { get; set; } = "";
        public string issued_token_type { get; set; } = "";

#pragma warning restore IDE1006 // Naming Styles
    }
}
 