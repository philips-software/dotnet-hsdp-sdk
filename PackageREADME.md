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

## Configuration & Usage

Each HSDP API that this SDK supports requires some configuration. Configuration and usage details can be found in the
github repository's [docs/](https://github.com/philips-software/dotnet-hsdp-sdk/tree/main/docs) folder per API 
(e.g. [CDR](https://github.com/philips-software/dotnet-hsdp-sdk/blob/main/docs/CDR.md)).

## Contact / Getting help

In case of questions about this SDK, please check with the [issue tracker](https://github.com/philips-software/dotnet-hsdp-sdk/issues)
if that is a known problem or feature request. And if that does not help, reach out to the maintainers:
- Aad Rijnberg <aad.rijnberg@philips.com>
- Ben Bierens <ben.bierens@philips.com>

## Contributing

We'd love for you to contribute to our source code and to make the repo even better than it is today! The guidelines 
we'd like you to follow are documented in our [github repo](https://github.com/philips-software/dotnet-hsdp-sdk/blob/main/CONTRIBUTING.md).
