using DotnetHsdpSdk.Internal;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetHsdpSdk.Utils
{
    public interface IHttpRequester
    {
        Task<T> HttpRequestWithBasicAuth<T>(IHsdpIamRequest requestContent, string path);
        Task<T> HttpRequestWithBearerAuth<T>(IHsdpIamRequest requestContent, string path, IIamToken token);
        Task HttpRequestWithBasicAuth(IHsdpIamRequest requestContent, string path);
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

        public async Task<T> HttpRequestWithBasicAuth<T>(IHsdpIamRequest requestContent, string path)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent.Content);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateWithBasicAuth(request, requestBody);

            return await http.HttpSendRequest<T>(request);
        }

        public async Task<T> HttpRequestWithBearerAuth<T>(IHsdpIamRequest requestContent, string path, IIamToken token)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent.Content);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateWithBearerAuth(request, requestBody, token);

            return await http.HttpSendRequest<T>(request);
        }

        public async Task HttpRequestWithBasicAuth(IHsdpIamRequest requestContent, string path)
        {
            using var requestBody = new FormUrlEncodedContent(requestContent.Content);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(configuration.IamEndpoint, path));
            DecorateWithBasicAuth(request, requestBody);

            await http.HttpSendRequest(request);
        }

        private void DecorateWithBasicAuth(HttpRequestMessage request, FormUrlEncodedContent requestBody)
        {
            DecorateRequest(request, requestBody);
            request.Headers.Add("Authorization", $"Basic {configuration.BasicAuthentication}");
        }

        private static void DecorateWithBearerAuth(HttpRequestMessage request, FormUrlEncodedContent requestBody, IIamToken token)
        {
            DecorateRequest(request, requestBody);
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
        }

        private static void DecorateRequest(HttpRequestMessage request, FormUrlEncodedContent requestBody)
        {
            request.Content = requestBody;
            request.Headers.Add("api-version", "2");
            request.Headers.Add("Accept", "application/json");
        }
    }
}
