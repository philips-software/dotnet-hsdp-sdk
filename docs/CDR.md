# CDR

Implemented calls:
- [x] Read resource by id
- [x] Read resource by version id
- [x] Search a resource
- [x] Create a resource
- [x] Perform a batch or transaction request
- [x] Delete a resource
- [x] Updeate a resource
- [x] Patch a resource

## HsdpCdr instantiation via dependency injection

When using the SDK in a project with dependency injection AND the application only needs 1 CDR configuration,
the HsdpCdr instance can be added to the DI container as follows:

```csharp
    public class Startup
    {
        ...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsdpCdr(services.FromAppSettings(Configuration, "CdrConfiguration"));
            ...
        }
    }
```

`FromAppSettings` requires a certain structure of the application's configuration file, see 
[Configuration](#configuration) for details.

## HsdpCdr instantiation via manual creation

For cases where no dependency injection is used or multiple CDR configurations are needed in the application
(e.g. different CDR instances), configurations and HsdpCdr instances can be created manually.

This is illustrated in next example, which also reads a resource by id.

```csharp
    var config = new HsdpCdrConfiguration("cdr url", "3.0", "application/fhir+json; charset=UTF-8");
    var cdr = new HsdpCdr(config);

    var token = ...; // from IAM user or service login
    var resource = await cdr.Read(new CdrReadRequest("Observation", ExistingObservationId), token);
```

## Configuration

The CDR configuration can be added to the application's appsettings.json, e.g.:

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
  "CdrConfiguration": {
    "url": "...",
    "fhirVersion": "...",
    "mediaType": "..."
  }
}
```

It can then be used in Startup.cs as follows:

```csharp
    var cdrUrl = Configuration["CdrConfiguration:url"];
    var fhirVersion = Configuration["CdrConfiguration:fhirVersion"];
    var mediaType = Configuration["CdrConfiguration:mediaType"];
    var cdrConfiguration = new HsdpCdrConfiguration(cdrUrl, fhirVersion, mediaType);
    ...
```
