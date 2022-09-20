# TDR

Implemented calls:
- [x] Search for DataItems based on a set of parameters
- [ ] Create a new DataItem
- [ ] Delete a DataItem
- [ ] Patch a DataItem
- [ ] Create new DataItems in a batch
- [ ] Create a new Contract
- [ ] Search for Contracts based on a set of parameters

## HsdpTdr instantiation via dependency injection

When using the SDK in a project with dependency injection AND the application only needs 1 TDR configuration,
the HsdpTdr instance can be added to the DI container as follows:

```csharp
    public class Startup
    {
        ...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsdpTdr(services.FromAppSettings(Configuration, "TdrConfiguration"));
            ...
        }
    }
```

`FromAppSettings` requires a certain structure of the application's configuration file, see 
[Configuration](#configuration) for details.

## HsdpTdr instantiation via manual creation

For cases where no dependency injection is used or multiple TDR configurations are needed in the application
(e.g. different TDR instances), configurations and HsdpTdr instances can be created manually.

This is illustrated in next example, which also performs a search for data items by providing the full url 
(including the query parameters for the request).

```csharp
    var config = new HsdpTdrConfiguration("tdr url");
    var tdr = new HsdpTdr(config);

    var token = ...; // from IAM user or service login
    var dataItems = await tdr.SearchDataItems(new TdrSearchDataRequestByUrl("full url with query parameters"), token);
```

## Configuration

The TDR configuration can be added to the application's appsettings.json, e.g.:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "TdrConfiguration": {
    "url": "..."
  }
}
```

It can then be used in Startup.cs as follows:

```csharp
    var tdrUrl = Configuration["TdrConfiguration:url"];
    var tdrConfiguration = new HsdpTdrConfiguration(tdrUrl);
    ...
```
