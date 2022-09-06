# HSDP SDK for Dotnet 6.0

Everything here is work in progress!

## IAM

Implemented calls:
- [x] User login
- [x] Service login
- [x] Refresh token
- [x] Revoke token
- [x] Introspect token
- [x] UserInfo for token
- [ ] Refresh session
- [ ] Terminate session
- [ ] Openid configuration
- [ ] jwks

### HsdpIam instantiation via dependency injection

When using the SDK in a project with dependency injection AND the application only needs 1 IAM configuration,
the HsdpIam instance can be added to the DI container as follows:

```csharp
    public class Startup
    {
        ...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsdpIam(new HsdpIamConfiguration(...,...,...));
            ...
        }
    }
```

### HsdpIam instantiation via manual creation

For cases where no dependency injection is used or multiple IAM configurations are needed in the application
(e.g. different IAM instances, or same IAM instance but with different client IDs/secrets), configurations 
and HsdpIam instances can be created manually.

This is illustrated in next example, which also performs a user login and other implemented methods.

```csharp
    var config = new HsdpIamConfiguration(new Uri("iam url"), "client id", "client secret");
    var iam = new HsdpIam(config);

    var token = await iam.UserLogin(new IamUserLoginRequest("username", "password"));
    var tokenMetadata = await iam.Introspect(token);
    var userInfo = await iam.GetUserInfo(token);
    
    var refreshedToken = await iam.RefreshToken(token);
    await iam.RevokeToken(refreshedToken);
```

### Service login

Next example shows how to log into IAM as a service: 
```csharp
    var config = new HsdpIamConfiguration(new Uri("iam url"), "client id", "client secret");
    var iam = new HsdpIam(config);

    var token = iam.ServiceLogin(new IamServiceLoginRequest("service key", "service audience", "service id"));
    ...
```
