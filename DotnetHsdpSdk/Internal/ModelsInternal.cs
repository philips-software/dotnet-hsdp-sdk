using System;
using System.Collections.Generic;
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
        public DateTime ExpiresUtc { get; private set; }
        public string TokenType { get; private set; }
        public string Scopes { get; private set; }
        public string IdToken { get; private set; }
        public string SignedToken { get; private set; }
        public string IssuedTokenType { get; private set; }
        public string? RefreshToken { get; private set; }

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
            ExpiresUtc = DateTime.MinValue;
            TokenType = string.Empty;
            Scopes = string.Empty;
            IdToken = string.Empty;
            SignedToken = string.Empty;
            IssuedTokenType = string.Empty;
            RefreshToken = string.Empty;
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

    internal class IntrospectResponse
    {
#pragma warning disable IDE1006 // Naming Styles

        public bool active { get; set; } = false;
        public string scope { get; set; } = "";
        public string client_id { get; set; } = "";
        public string username { get; set; } = "";
        public string token_type { get; set; } = "";
        public long exp { get; set; }
        public string? sub { get; set; } = null;
        public string iss { get; set; } = "";
        public string identity_type { get; set; } = "";
        public string device_type { get; set; } = "";
        public HsdpOrganizations? organizations { get; set; } = null;
        public string token_type_hint { get; set; } = "";
        public string client_organization_id { get; set; } = "";
        public HsdpActor? act { get; set; } = null;

#pragma warning restore IDE1006 // Naming Styles
    }

    public class HsdpOrganizations
    {
#pragma warning disable IDE1006 // Naming Styles

        public string managingOrganization { get; set; } = "";
        public List<HsdpOrganization> organizationList { get; set; } = new();

#pragma warning restore IDE1006 // Naming Styles
    }

    public class HsdpOrganization
    {
#pragma warning disable IDE1006 // Naming Styles

        public string organizationId { get; set; } = "";
        public string organizationName { get; set; } = "";
        public bool? disabled { get; set; }
        public List<string> permissions { get; set; } = new();
        public List<string> effectivePermissions { get; set; } = new();
        public List<string> roles { get; set; } = new();
        public List<string> groups { get; set; } = new();
        
#pragma warning restore IDE1006 // Naming Styles
    }

    public class HsdpActor
    {
#pragma warning disable IDE1006 // Naming Styles

        public string sub { get; set; } = "";
        
#pragma warning restore IDE1006 // Naming Styles
    }

    internal class UserInfoResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        
        public string sub { get; set; } = "";
        public string name { get; set; } = "";
        public string given_name { get; set; } = "";
        public string family_name { get; set; } = "";
        public string email { get; set; } = "";
        public AddressClaim? address { get; set; } = null;
        public long updated_at { get; set; } = 0;

#pragma warning restore IDE1006 // Naming Styles
    }

    internal class AddressClaim
    {
#pragma warning disable IDE1006 // Naming Styles
        
        public string formatted { get; set; } = "";
        public string street_address { get; set; } = "";
        public string postal_code { get; set; } = "";

#pragma warning restore IDE1006 // Naming Styles
    }
}
 