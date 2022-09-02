using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DotnetHsdpSdk.Internal;
using DotnetHsdpSdk.Utils;

[assembly: InternalsVisibleTo("DotnetHsdpSdkTests")]
namespace DotnetHsdpSdk.API
{
    public class HsdpIam : IHsdpIam
    {
        private readonly IHttpRequester http;
        private readonly IHsdpIamRequestFactory hsdpIamRequestFactory;
        
        private const string TokenPath = "authorize/oauth2/token";
        private const string RevokePath = "authorize/oauth2/revoke";
        private const string IntrospectPath = "authorize/oauth2/introspect";
        private const string UserInfoPath = "authorize/oauth2/userinfo";

        public HsdpIam(HsdpIamConfiguration configuration)
            : this(new HttpRequester(configuration, new Http()), new HsdpIamRequestFactory())
        {
        }

        internal HsdpIam(IHttpRequester http, IHsdpIamRequestFactory hsdpIamRequestFactory)
        {
            this.http = http;
            this.hsdpIamRequestFactory = hsdpIamRequestFactory;
        }

        public async Task<IIamToken> UserLogin(IamUserLoginRequest userLoginRequest)
        {
            const string apiVersion = "2";
            var requestContent = hsdpIamRequestFactory.CreateUserLoginRequestContent(userLoginRequest);
            var tokenResponse = await http.HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath, apiVersion);
            return CreateIamToken(tokenResponse);
        }

        public async Task<IIamToken> ServiceLogin(IamServiceLoginRequest serviceLoginRequest)
        {
            const string apiVersion = "2";
            var requestContent = hsdpIamRequestFactory.CreateServiceLoginRequestContent(serviceLoginRequest);
            var tokenResponse = await http.HttpRequestWithoutAuth<TokenResponse>(requestContent, TokenPath, apiVersion);
            return CreateIamToken(tokenResponse);
        }

        public async Task<IIamToken> RefreshToken(IIamToken token)
        {
            const string apiVersion = "2";
            ValidateToken(token);
            if (string.IsNullOrEmpty(token.RefreshToken)) throw new InvalidOperationException("Provided token cannot be refreshed. (RefreshToken is null or empty.)");

            var requestContent = hsdpIamRequestFactory.CreateRefreshRequestContent(token);
            var tokenResponse = await http.HttpRequestWithBasicAuth<TokenResponse>(requestContent, TokenPath, apiVersion);
            return CreateIamToken(tokenResponse);
        }

        public async Task RevokeToken(IIamToken token)
        {
            const string apiVersion = "2";
            ValidateToken(token);

            var t = token as IamToken;
            if (t == null) throw new InvalidOperationException("Provided token is not of expected type.");

            var requestContent = hsdpIamRequestFactory.CreateRevokeRequestContent(token);
            await http.HttpRequestWithBasicAuth(requestContent, RevokePath, apiVersion);
            
            t.MarkAsRevoked();
        }

        public async Task<TokenMetadata> Introspect(IIamToken token)
        {
            const string apiVersion = "4";
            ValidateToken(token);

            var requestContent = hsdpIamRequestFactory.CreateIntrospectRequestContent(token);
            var introspectResponse = await http.HttpRequestWithBasicAuth<IntrospectResponse>(requestContent, IntrospectPath, apiVersion);
            return CreateTokenMetadata(introspectResponse);
        }

        public async Task<HsdpUserInfo> GetUserInfo(IIamToken token)
        {
            const string apiVersion = "2";
            ValidateToken(token);
            var requestContent = hsdpIamRequestFactory.CreateEmptyRequestContent();
            var getUserInfoResponse = await http.HttpRequestWithBearerAuth<UserInfoResponse>(requestContent, UserInfoPath, HttpMethod.Get, apiVersion, token);
            return CreateHsdpUserInfo(getUserInfoResponse);
        }

        private static IamToken CreateIamToken(TokenResponse tokenResponse)
        {
            return new IamToken(
                accessToken: tokenResponse.access_token,
                expiresUtc: DateTime.UtcNow.AddMinutes(tokenResponse.expires_in),
                tokenType: tokenResponse.token_type,
                scopes: tokenResponse.scopes,
                idToken: tokenResponse.id_token,
                signedToken: tokenResponse.signed_token,
                issuedTokenType: tokenResponse.issued_token_type,
                refreshToken: tokenResponse.refresh_token
            );
        }

        private static TokenMetadata CreateTokenMetadata(IntrospectResponse introspectResponse)
        {
            return new TokenMetadata
            {
                IsActive = introspectResponse.active,
                Scopes = introspectResponse.scope,
                ClientId = introspectResponse.client_id,
                UserName = introspectResponse.username,
                TokenType = introspectResponse.token_type,
                ExpirationTimeInEpochSeconds = introspectResponse.exp,
                Subject = introspectResponse.sub,
                Issuer = introspectResponse.iss,
                IdentityType = introspectResponse.identity_type,
                DeviceType = introspectResponse.device_type,
                Organizations = introspectResponse.organizations != null ? CreateOrganizations(introspectResponse.organizations) : null,
                TokenTypeHint = introspectResponse.token_type_hint,
                ClientOrganizationId = introspectResponse.client_organization_id,
                Actor = introspectResponse.act != null ? CreateActor(introspectResponse.act) : null
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

        private static HsdpUserInfo CreateHsdpUserInfo(UserInfoResponse userInfoResponse)
        {
            return new HsdpUserInfo
            {
                Subject = userInfoResponse.sub,
                Name = userInfoResponse.name,
                GivenName = userInfoResponse.given_name,
                FamilyName = userInfoResponse.family_name,
                Email = userInfoResponse.email,
                Address = userInfoResponse.address != null ? CreateAddress(userInfoResponse.address) : null,
                UpdatedAtInEpochSeconds = userInfoResponse.updated_at
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
}
