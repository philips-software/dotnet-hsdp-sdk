using DotnetHsdpSdk.Internal;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetHsdpSdk.Utils
{
    public interface IHttpRequester
    {
        Task<T> HttpRequestWithoutAuth<T>(IHsdpIamRequest requestContent, string path, string version);
        Task<T> HttpRequestWithBasicAuth<T>(IHsdpIamRequest requestContent, string path, string version);
        Task<T> HttpRequestWithBearerAuth<T>(IHsdpIamRequest requestContent, string path, HttpMethod method, string version, IIamToken token);
        Task HttpRequestWithBasicAuth(IHsdpIamRequest requestContent, string path, string version);
    }

    // Todo - refactor for general-purpose use when other APIs are being developed.
    internal class HttpRequester : IHttpRequester
    {
        private readonly HsdpIamConfiguration configuration;
        private readonly IHttp http;

        public HttpRequester(HsdpIamConfiguration configuration, IHttp http)
        {
            this.configuration = configuration;
            this.http = http;
        }

        public async Task<T> HttpRequestWithoutAuth<T>(IHsdpIamRequest requestContent, string path, string version)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent.Content);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateRequest(request, requestBody, version);

            return await http.HttpSendRequest<T>(request);
        }

        public async Task<T> HttpRequestWithBasicAuth<T>(IHsdpIamRequest requestContent, string path, string version)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent.Content);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateWithBasicAuth(request, requestBody, version);

            return await http.HttpSendRequest<T>(request);
        }

        public async Task<T> HttpRequestWithBearerAuth<T>(IHsdpIamRequest requestContent, string path, HttpMethod method, string version, IIamToken token)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent.Content);
            using var request = new HttpRequestMessage(method, new Uri(configuration.IamEndpoint, path));
            DecorateWithBearerAuth(request, requestBody, token, version);

            return await http.HttpSendRequest<T>(request);
        }

        public async Task HttpRequestWithBasicAuth(IHsdpIamRequest requestContent, string path, string version)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent.Content);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateWithBasicAuth(request, requestBody, version);

            await http.HttpSendRequest(request);
        }

        private void DecorateWithBasicAuth(HttpRequestMessage request, FormUrlEncodedContent requestBody, string version)
        {
            DecorateRequest(request, requestBody, version);
            request.Headers.Add("Authorization", $"Basic {configuration.BasicAuthentication}");
        }

        private static void DecorateWithBearerAuth(HttpRequestMessage request, FormUrlEncodedContent requestBody, IIamToken token, string version)
        {
            DecorateRequest(request, requestBody, version);
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
        }

        private static void DecorateRequest(HttpRequestMessage request, FormUrlEncodedContent requestBody, string version)
        {
            request.Content = requestBody;
            request.Headers.Add("api-version", version);
            request.Headers.Add("Accept", "application/json");
        }
    }
}
