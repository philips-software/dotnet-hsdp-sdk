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
        public List<Organization> OrganizationList { get; set; } = new();
    }

    public class Organization
    {
        public string OrganizationId { get; set; } = "";
        public string OrganizationName { get; set; } = "";
        public bool? Disabled { get; set; }
        public List<string> Permissions { get; set; } = new();
        public List<string> EffectivePermissions { get; set; } = new();
        public List<string> Roles { get; set; } = new();
        public List<string> Groups { get; set; } = new();
    }

    public class TokenMetadata
    {
        public bool IsActive { get; set; }
        public string Scopes { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string TokenType { get; set; } = "";
        public long ExpirationTimeInEpochSeconds { get; set; }
        public string? Subject { get; set; } = null;
        public string Issuer { get; set; } = "";
        public string IdentityType { get; set; } = "";
        public string DeviceType { get; set; } = "";
        public Organizations? Organizations { get; set; } = null;
        public string TokenTypeHint { get; set; } = "";
        public string ClientOrganizationId { get; set; } = "";
        public Actor? Actor { get; set; } = null;
    }

    public class Actor
    {
        public string Sub { get; set; } = "";
    }

    public class HsdpUserInfo
    {
        public string Subject { get; set; } = "";
        public string Name { get; set; } = "";
        public string GivenName { get; set; } = "";
        public string FamilyName { get; set; } = "";
        public string Email { get; set; } = "";
        public Address? Address { get; set; }
        public long UpdatedAtInEpochSeconds { get; set; }
    }

    public class Address
    {
        public string Formatted { get; set; } = "";
        public string StreetAddress { get; set; } = "";
        public string PostalCode { get; set; } = "";
    }
}
