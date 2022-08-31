using System;
using System.Threading.Tasks;

namespace DotnetHsdpSdk
{
    public interface IHsdpIam
    {
        Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest);
    }

    public interface IIamToken
    {
        string AccessToken { get; }
        DateTime ExpiresUtc { get; }
    }
}
