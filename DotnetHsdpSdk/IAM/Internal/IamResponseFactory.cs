using System.Linq;
using System.Security.Authentication;
using System.Text.Json;
using DotnetHsdpSdk.Utils;

namespace DotnetHsdpSdk.IAM.Internal;

public interface IIamResponseFactory
{
    IIamToken CreateIamToken(IHsdpResponse<string> hsdpResponse);
    IamTokenMetadata CreateIamTokenMetadata(IHsdpResponse<string> hsdpResponse);
    IamUserInfo CreateIamUserInfo(IHsdpResponse<string> hsdpResponse);
}

internal class IamResponseFactory : IIamResponseFactory
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public IamResponseFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public IIamToken CreateIamToken(IHsdpResponse<string> hsdpResponse)
    {
        if (!hsdpResponse.StatusCode.IsSuccess()) throw new AuthenticationException(hsdpResponse.Body);

        var response = Deserialize<TokenResponse>(hsdpResponse.Body);
        return new IamToken(
            response.access_token,
            _dateTimeProvider.UtcNow.AddMinutes(response.expires_in),
            response.token_type,
            response.scopes,
            response.id_token,
            response.signed_token,
            response.issued_token_type,
            response.refresh_token
        );
    }

    public IamTokenMetadata CreateIamTokenMetadata(IHsdpResponse<string> hsdpResponse)
    {
        if (!hsdpResponse.StatusCode.IsSuccess()) throw new AuthenticationException(hsdpResponse.Body);

        var response = Deserialize<IntrospectResponse>(hsdpResponse.Body);
        return new IamTokenMetadata
        {
            IsActive = response.active,
            Scopes = response.scope,
            ClientId = response.client_id,
            UserName = response.username,
            TokenType = response.token_type,
            ExpirationTimeInEpochSeconds = response.exp,
            Subject = response.sub,
            Issuer = response.iss,
            IdentityType = response.identity_type,
            DeviceType = response.device_type,
            Organizations = response.organizations != null
                ? CreateOrganizations(response.organizations)
                : null,
            TokenTypeHint = response.token_type_hint,
            ClientOrganizationId = response.client_organization_id,
            Actor = response.act != null ? CreateActor(response.act) : null
        };
    }

    public IamUserInfo CreateIamUserInfo(IHsdpResponse<string> hsdpResponse)
    {
        if (!hsdpResponse.StatusCode.IsSuccess()) throw new AuthenticationException(hsdpResponse.Body);

        var response = Deserialize<UserInfoResponse>(hsdpResponse.Body);
        return new IamUserInfo
        {
            Subject = response.sub,
            Name = response.name,
            GivenName = response.given_name,
            FamilyName = response.family_name,
            Email = response.email,
            Address = response.address != null ? CreateAddress(response.address) : null,
            UpdatedAtInEpochSeconds = response.updated_at
        };
    }

    private static T Deserialize<T>(string? body)
    {
        if (body == null) throw new HsdpRequestException(InternalApiError.StatusCode, "Response body is missing");
        try
        {
            var resource = JsonSerializer.Deserialize<T>(body);
            if (resource == null)
                throw new HsdpRequestException(InternalApiError.StatusCode, "Deserialized body is null");

            return resource;
        }
        catch (JsonException e)
        {
            throw new HsdpRequestException(InternalApiError.StatusCode, "Invalid JSON", e);
        }
    }

    private static Organizations CreateOrganizations(HsdpOrganizations organizations)
    {
        return new Organizations
        {
            ManagingOrganization = organizations.managingOrganization,
            OrganizationList = organizations.organizationList.Select(CreateOrganization).ToList()
        };
    }

    private static Organization CreateOrganization(HsdpOrganization organization)
    {
        return new Organization
        {
            OrganizationId = organization.organizationId,
            OrganizationName = organization.organizationName,
            Disabled = organization.disabled,
            Permissions = organization.permissions,
            EffectivePermissions = organization.effectivePermissions,
            Roles = organization.roles,
            Groups = organization.groups
        };
    }

    private static Actor CreateActor(HsdpActor actor)
    {
        return new Actor
        {
            Sub = actor.sub
        };
    }

    private static Address CreateAddress(AddressClaim addressClaim)
    {
        return new Address
        {
            Formatted = addressClaim.formatted,
            StreetAddress = addressClaim.street_address,
            PostalCode = addressClaim.postal_code
        };
    }
}
