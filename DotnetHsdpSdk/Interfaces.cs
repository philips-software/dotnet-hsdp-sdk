using System;
using System.Threading.Tasks;

namespace DotnetHsdpSdk
{
    public interface IHsdpIam
    {
        Task<IIamToken> UserLogin(IamUserLoginRequest userLoginRequest);
        Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest);
        Task<IIamToken> RefreshToken(IIamToken token);
        Task RevokeToken(IIamToken token);
        Task<TokenMetadata> Introspect(IIamToken token);
        Task<HsdpUserInfo> GetUserInfo(IIamToken token);
    }

    public interface IIamToken
    {
        string AccessToken { get; }
        DateTime ExpiresUtc { get; }
        string TokenType { get; }
        string Scopes { get; }
        string IdToken { get; }
        string SignedToken { get; }
        string IssuedTokenType { get; }
        string? RefreshToken { get; }

        bool IsExpired();
        bool IsValid();
    }
}
