# HSDP SDK for Dotnet 6.0

This project provides a Software Development Kit that makes interaction with Philips HSDP APIs easy for a DotNet programmer.
Instead of having to worry about composing the correct HTTP requests (required url, headers, body, query parameters, parsing 
responses), the user interacts with statically typed data structures, giving autocompletion and field name discovery of the 
request and response data structures.

- **Technology stack**: the library is intended for use in any DotNet project that needs to interact with HSDP APIs. 
  The programming language is C#.
- **Status**: the software is still in early development, only supporting a few of the HSDP APIs.

## Dependencies

The SDK can be used in any DotNet project. It is required to have a DotNet eco system installed on your system, e.g. Microsoft 
Visual Studio or Jetbrains Rider. Library dependencies will be retrieved via NuGet. 

## Installation

The SDK must be added to the installed packages for your app via the package manager in your IDE.

## Configuration

Each HSDP API that this SDK supports requires some configuration. Details can be found in the [docs/](/docs) folder per API 
(e.g. [CDR](/docs/CDR.md#configuration)).

## Usage

Usage is documented per HSDP API in the [/docs](/docs) folder. Overview of the APIs that have been (partially) implemented:
- [IAM OAuth2](docs/IAM-OAuth2.md)
- [TDR](docs/TDR.md)
- [CDR](docs/CDR.md)

## How to test the software

The SDK is supplemented with unit tests that are available in the included `DotNetHsdpSdkTests` project.
In e.g. Jetbrains Rider, the test project can be right clicked and `Run unit tests` can be selected.
Alternatively, the tests can be run from the command line, using:

```sh
$ dotnet test
```

## Known issues

At this moment, there are no known issues.

## Contact / Getting help

In case of questions about this SDK, please check with the issue tracker if that is a known problem or feature request.
And if that does not help, reach out to the [maintainers](MAINTAINERS.md).

## License

See [LICENSE.md](LICENSE.md).

## Credits and references

This SDK used the [Kotlin HSDP SDK](https://github.com/philips-software/kotlin-hsdp-sdk) as an inspiration.
