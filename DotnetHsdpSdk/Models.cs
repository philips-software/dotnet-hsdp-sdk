using DotnetHsdpSdk.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DotnetHsdpSdk
{
    public class HsdpIamConfiguration
    {
        public HsdpIamConfiguration(Uri iamEndpoint, string clientId, string clientSecret)
        {
            Validate.NotNull(iamEndpoint, nameof(iamEndpoint));
            Validate.NotNullOrEmpty(clientId, nameof(clientId));
            Validate.NotNull(clientSecret, nameof(clientSecret));

            IamEndpoint = iamEndpoint;

            BasicAuthentication = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
        }

        public Uri IamEndpoint { get; }
        internal string BasicAuthentication { get; }
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
        public IamServiceLoginRequest(string serviceKey, string serviceAudience, string serviceId)
        {
            Validate.NotNullOrEmpty(serviceKey, nameof(serviceKey));
            Validate.NotNullOrEmpty(serviceAudience, nameof(serviceAudience));
            Validate.NotNullOrEmpty(serviceId, nameof(serviceId));

            ServiceKey = serviceKey;
            ServiceAudience = serviceAudience;
            ServiceId = serviceId;
        }

        public string ServiceKey { get; }
        public string ServiceAudience { get; }
        public string ServiceId { get; }
    }

    public class Organizations
    {
        public string ManagingOrganization { get; set; } = "";
        public List<Organization> OrganizationList { get; set; } = new List<Organization>();
    }

    public class Organization
    {
        public string OrganizationId { get; set; } = "";
        public string OrganizationName { get; set; } = "";
        public bool? Disabled { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public List<string> EffectivePermissions { get; set; } = new List<string>();
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Groups { get; set; } = new List<string>();
    }

    public class TokenMetadata
    {
        public bool IsActive { get; set; }
        public string Scopes { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string TokenType { get; set; } = "";
        public long ExpirationTimeInEpochSeconds { get; set; }
        public string Subject { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string IdentityType { get; set; } = "";
        public string DeviceType { get; set; } = "";
        public Organizations Organizations { get; set; } = new Organizations();
        public string TokenTypeHint { get; set; } = "";
        public string ClientOrganizationId { get; set; } = "";
        public Actor Actor { get; set; } = new Actor();
    }

    public class Actor
    {
        public string Sub { get; set; } = "";
    }

    public class HsdpUserInfo
    {
        [JsonPropertyName("sub")]
        public string Subject { get; set; } = "";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = "";
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = "";
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("updated_at")]
        public long UpdatedAtInEpochSeconds { get; set; }
    }
}
