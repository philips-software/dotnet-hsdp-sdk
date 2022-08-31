using DotnetHsdpSdk.Utils;
using System;

namespace DotnetHsdpSdk
{
    public class HsdpIamConfiguration
    {
        public HsdpIamConfiguration(Uri iamEndpoint)
        {
            Validate.NotNull(iamEndpoint, nameof(iamEndpoint));

            IamEndpoint = iamEndpoint;
        }

        public Uri IamEndpoint { get; }
    }

    public class IamUserLoginRequest
    {
        public IamUserLoginRequest(string username, string password)
        {
            Validate.NotNullOrEmpty(username, nameof(username));
            Validate.NotNullOrEmpty(password, nameof(password));

            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }


    public class IamServiceLoginRequest
    {
        public IamServiceLoginRequest(string serviceKey, string serviceAudience, string serviceId, bool forceRefetch = false)
        {
            Validate.NotNullOrEmpty(serviceKey, nameof(serviceKey));
            Validate.NotNullOrEmpty(serviceAudience, nameof(serviceAudience));
            Validate.NotNullOrEmpty(serviceId, nameof(serviceId));

            ServiceKey = serviceKey;
            ServiceAudience = serviceAudience;
            ServiceId = serviceId;
            ForceRefetch = forceRefetch;
        }

        public string ServiceKey { get; }
        public string ServiceAudience { get; }
        public string ServiceId { get; }
        public bool ForceRefetch { get; }
    }
}
