using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Threading.Tasks;
using DotnetHsdpSdk.IAM.Internal;
using DotnetHsdpSdk.Utils;

[assembly: InternalsVisibleTo("DotnetHsdpSdkTests")]

namespace DotnetHsdpSdk.IAM;

public class HsdpIam : IHsdpIam
{
    private readonly IRequestFactory _requestFactory;
    private readonly IHttpRequester _http;

    public HsdpIam(HsdpIamConfiguration configuration)
        : this(new HttpRequester(), new RequestFactory(configuration))
    {
    }

    internal HsdpIam(IHttpRequester http, IRequestFactory requestFactory)
    {
        _http = http;
        _requestFactory = requestFactory;
    }

    public async Task<IIamToken> UserLogin(IamUserLoginRequest userLoginRequest)
    {
        var request = _requestFactory.CreateUserLoginRequest(userLoginRequest);
        return await HandleRequest(request, "User login", async req =>
        {
            var response = await _http.HttpRequest<TokenResponse>(req);
            return CreateIamToken(response);
        });
    }

    public async Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest)
    {
        var request = _requestFactory.CreateServiceLoginRequest(serviceLoginRequest);
        return await HandleRequest(request, "Service login", async req =>
        {
            var response = await _http.HttpRequest<TokenResponse>(req);
            return CreateIamToken(response);
        });
    }

    public async Task<IIamToken> RefreshToken(IIamToken token)
    {
        ValidateToken(token);
        if (string.IsNullOrEmpty(token.RefreshToken))
            throw new InvalidOperationException("Provided token cannot be refreshed. (RefreshToken is null or empty.)");

        var request = _requestFactory.CreateRefreshRequest(token);
        return await HandleRequest(request, "Token refresh", async req =>
        {
            var response = await _http.HttpRequest<TokenResponse>(req);
            return CreateIamToken(response);
        });
    }

    public async Task RevokeToken(IIamToken token)
    {
        ValidateToken(token);
        var t = token as IamToken;
        if (t == null) throw new InvalidOperationException("Provided token is not of expected type.");
        var request = _requestFactory.CreateRevokeRequest(token);

        await HandleRequest(request, "Token revocation", async req =>
        {
            await _http.HttpRequest(req);
            t.MarkAsRevoked();
        });
    }

    public async Task<TokenMetadata> Introspect(IIamToken token)
    {
        ValidateToken(token);
        var request = _requestFactory.CreateIntrospectRequest(token);

        return await HandleRequest(request, "Token introspection", async req =>
        {
            var response = await _http.HttpRequest<IntrospectResponse>(req);
            return CreateTokenMetadata(response);
        });
    }

    public async Task<HsdpUserInfo> GetUserInfo(IIamToken token)
    {
        ValidateToken(token);
        var request = _requestFactory.CreateUserInfoRequest(token);

        return await HandleRequest(request, "UserInfo retrieval", async req =>
        {
            var response = await _http.HttpRequest<UserInfoResponse>(req);
            return CreateHsdpUserInfo(response);
        });
    }

    private static async Task<T> HandleRequest<T>(
        IHsdpRequest request,
        string actionDescription,
        Func<IHsdpRequest, Task<T>> requestAndConvert)
    {
        try
        {
            return await requestAndConvert(request);
        }
        catch (HsdpRequestException e)
        {
            Console.WriteLine($"Token refresh failed {e.Message}");
            throw new AuthenticationException($"{actionDescription} failed", e);
        }
    }

    private static async Task HandleRequest(
        IHsdpRequest request,
        string actionDescription,
        Func<IHsdpRequest, Task> requestAndConvert)
    {
        try
        {
            await requestAndConvert(request);
        }
        catch (HsdpRequestException e)
        {
            Console.WriteLine($"Token refresh failed {e.Message}");
            throw new AuthenticationException($"{actionDescription} failed", e);
        }
    }

    private static IamToken CreateIamToken(IHsdpResponse<TokenResponse> tokenResponse)
    {
        var response = tokenResponse.Body!;
        return new IamToken(
            response.access_token,
            DateTime.UtcNow.AddMinutes(response.expires_in),
            response.token_type,
            response.scopes,
            response.id_token,
            response.signed_token,
            response.issued_token_type,
            response.refresh_token
        );
    }

    private static TokenMetadata CreateTokenMetadata(IHsdpResponse<IntrospectResponse> introspectResponse)
    {
        var response = introspectResponse.Body!;
        return new TokenMetadata
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

    private static HsdpUserInfo CreateHsdpUserInfo(IHsdpResponse<UserInfoResponse> userInfoResponse)
    {
        var response = userInfoResponse.Body!;
        return new HsdpUserInfo
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

    private static Address CreateAddress(AddressClaim addressClaim)
    {
        return new Address
        {
            Formatted = addressClaim.formatted,
            StreetAddress = addressClaim.street_address,
            PostalCode = addressClaim.postal_code
        };
    }

    private static void ValidateToken(IIamToken token)
    {
        Validate.NotNull(token, nameof(token));
        if (!token.IsValid()) throw new InvalidOperationException("Provided token is not valid.");
    }
}