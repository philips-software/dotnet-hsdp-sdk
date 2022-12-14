using System;
using System.Threading.Tasks;

namespace DotnetHsdpSdk.IAM;

public interface IHsdpIam
{
    Task<IIamToken> UserLogin(IamUserLoginRequest userLoginRequest);
    Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest);
    Task<IIamToken> RefreshToken(IIamToken token);
    Task RevokeToken(IIamToken token);
    Task<IamTokenMetadata> Introspect(IIamToken token);
    Task<IamUserInfo> GetUserInfo(IIamToken token);
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
