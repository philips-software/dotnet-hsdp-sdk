using System;

namespace DotnetHsdpSdk
{
    public class HsdpIamConfiguration
    {
        public HsdpIamConfiguration(Uri iamEndpoint)
        {
            IamEndpoint = iamEndpoint;
        }

        public Uri IamEndpoint { get; }
    }

    public class IamServiceLoginRequest
    {
        public IamServiceLoginRequest(string serviceKey, string serviceAudience, string serviceId, bool forceRefetch = false)
        {
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
