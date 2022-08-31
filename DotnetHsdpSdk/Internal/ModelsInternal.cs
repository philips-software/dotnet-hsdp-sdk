using System;

namespace DotnetHsdpSdk.Internal
{
    internal class IamToken : IIamToken
    {
        public IamToken(string accessToken, DateTime expiresUtc)
        {
            AccessToken = accessToken;
            ExpiresUtc = expiresUtc;
        }

        public string AccessToken { get; }
        public DateTime ExpiresUtc { get; }
    }

    internal class TokenResponse
    {
        // ReSharper disable once InconsistentNaming
        public string access_token { get; set; } = "";
        // ReSharper disable once InconsistentNaming
        public string token_type { get; set; } = "";
        // ReSharper disable once InconsistentNaming
        public int expires_in { get; set; }
    }
}
