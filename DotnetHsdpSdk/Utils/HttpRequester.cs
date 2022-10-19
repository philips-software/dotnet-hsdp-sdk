using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotnetHsdpSdk.Utils;

public interface IHttpRequester
{
    Task<IHsdpResponse> HttpRequest(IHsdpRequest request);
    Task<IHsdpResponse<T>> HttpRequest<T>(IHsdpRequest request) where T : class;
}

public class HttpRequester : IHttpRequester
{
    public async Task<IHsdpResponse> HttpRequest(IHsdpRequest request)
    {
        var responseMessage = await BuildAndExecuteRequest(request).ConfigureAwait(false);
        var headers = GetHeaders(responseMessage);

        return new HsdpResponse(responseMessage.StatusCode, headers);
    }

    public async Task<IHsdpResponse<T>> HttpRequest<T>(IHsdpRequest request) where T : class
    {
        // NOTE: only uncomment LogRequest and LogResponse when debugging locally!!!
        // await LogRequest(request);

        var responseMessage = await BuildAndExecuteRequest(request).ConfigureAwait(false);
        var headers = GetHeaders(responseMessage);

        try
        {
            // await LogResponse(responseMessage);

            var responseBody = typeof(T) == typeof(string)
                ? await responseMessage.Content.ReadAsStringAsync() as T
                : await responseMessage.Content.ReadFromJsonAsync<T>()
                  ?? throw new HsdpRequestException(500, "Response body should not be null");
            return new HsdpResponse<T>(responseMessage.StatusCode, headers, responseBody!);
        }
        catch (NotSupportedException e)
        {
            throw new HsdpRequestException(500, "Invalid content type", e);
        }
        catch (JsonException e)
        {
            throw new HsdpRequestException(500, "Invalid JSON", e);
        }
    }

    private static async Task LogRequest(IHsdpRequest request)
    {
        Console.WriteLine($"{request.Method.Method.ToUpper()}   {request.Path}");
        foreach (var (key, value) in request.Headers) Console.WriteLine($"Header {key} : {value}");
        if (request.Content != null)
            foreach (var (key, value) in request.Content.Headers)
                Console.WriteLine($"Header {key} : {string.Join(",", value)}");
        foreach (var (key, value) in request.QueryParameters) Console.WriteLine($"QueryParameter {key} : {value}");
        if (request.Content is JsonContent) await LogContent("Request", request.Content);
    }

    private static async Task LogResponse(HttpResponseMessage response)
    {
        Console.WriteLine($"StatusCode {response.StatusCode}");
        foreach (var (key, value) in response.Headers) Console.WriteLine($"Header {key} : {string.Join(",", value)}");
        await LogContent("Response", response.Content);
    }

    private static async Task LogContent(string description, HttpContent content)
    {
        var jsonString = await content.ReadAsStringAsync();
        Console.WriteLine($"{description} content = {jsonString}");
    }

    private static async Task<HttpResponseMessage> BuildAndExecuteRequest(IHsdpRequest request)
    {
        var uri = ComposeUriFromPathAndQueryParameters(request.Path, request.QueryParameters);
        var requestMessage = new HttpRequestMessage(request.Method, uri);
        foreach (var (key, value) in request.Headers) requestMessage.Headers.Add(key, value);
        requestMessage.Content = request.Content;

        using var client = new HttpClient();
        return await client.SendAsync(requestMessage);
    }

    private static List<KeyValuePair<string, string>> GetHeaders(HttpResponseMessage responseMessage)
    {
        var keyValuePairs = responseMessage.Headers
            .Select(pair => KeyValuePair.Create(pair.Key, pair.Value.FirstOrDefault() ?? ""))
            .ToList();
        // LastModified not part of the responseMessage.Headers; we need to get that separately from the Content.Headers
        if (responseMessage.Content.Headers.LastModified != null)
            keyValuePairs.Add(KeyValuePair.Create("Last-Modified",
                responseMessage.Content.Headers.LastModified.ToString()!));

        return keyValuePairs;
    }

    private static Uri ComposeUriFromPathAndQueryParameters(Uri path,
        List<KeyValuePair<string, string>> queryParameters)
    {
        var builder = new UriBuilder(path.GetLeftPart(UriPartial.Path))
        {
            Port = -1
        };
        var queryBuilder = new StringBuilder(path.Query);
        foreach (var (key, value) in queryParameters)
        {
            queryBuilder.Append(queryBuilder.Length == 0 ? "?" : "&");
            queryBuilder.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        builder.Query = queryBuilder.ToString();
        return new Uri(builder.ToString());
    }
}
