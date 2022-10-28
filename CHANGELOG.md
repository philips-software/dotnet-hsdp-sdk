# CHANGELOG

v0.1.4
- Fix issue with a token that has potentially expired, when there is a time between the token validation and the use of the token.
- Fix issue with returned TDR search result, where link is not always returned (e.g. when also supplying _id in the query parameters)

v0.1.3
- Fix issue for a path that already contains query parameters

v0.1.2
- Fix issue with special characters in URL query parameters

v0.1.1
- Add package readme for nuget

v0.1.0
- Initial version
