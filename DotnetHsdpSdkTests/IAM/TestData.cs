using DotnetHsdpSdk.IAM.Internal;

namespace DotnetHsdpSdkTests.IAM;

internal static class TestData
{
    public static readonly TokenResponse TokenResponse = new TokenResponse
    {
        access_token = "accessToken",
        expires_in = 60,
        token_type = "Bearer",
        refresh_token = "refreshToken",
        scopes = "scopes",
        id_token = "idToken",
        signed_token = "signedToken",
        issued_token_type = "issuedTokenType",
    };

    public static IntrospectResponse IntrospectResponse = new IntrospectResponse
    {
        active = true,
        scope = "scope",
        client_id = "clientId",
        username = "username",
        token_type = "tokenType",
        exp = 60,
        sub = "sub",
        iss = "iss",
        identity_type = "identityType",
        device_type = "deviceType",
        organizations = new HsdpOrganizations
        {
            managingOrganization = "managingOrganization",
            organizationList = new List<HsdpOrganization>
            {
                new()
                {
                    organizationId = "organizationId",
                    organizationName = "organizationName",
                    disabled = false,
                    permissions = new List<string> {"p1", "p2"},
                    effectivePermissions = new List<string> {"p1"},
                    roles = new List<string> { "r1" },
                    groups = new List<string> { "g1", "g2" },
                }
            }
        },
        token_type_hint = "tokenTypeHint",
        client_organization_id = "clientOrganizationId",
        act = new HsdpActor
        {
            sub = "act"
        }
    };
}
