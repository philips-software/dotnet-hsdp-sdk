using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DotnetHsdpSdk.Utils;

public interface IHttpRequester
{
    Task<IHsdpResponse> HttpRequest(IHsdpRequest request);
    Task<IHsdpResponse<T>> HttpRequest<T>(IHsdpRequest request) where T:class;
}

public class HttpRequester : IHttpRequester
{
    public async Task<IHsdpResponse> HttpRequest(IHsdpRequest request)
    {
        var responseMessage = await BuildAndExecuteRequest(request);
        var keyValuePairs = responseMessage.Headers
            .Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.FirstOrDefault() ?? ""))
            .ToList();

        return new HsdpResponse(keyValuePairs);
    }

    public async Task<IHsdpResponse<T>> HttpRequest<T>(IHsdpRequest request) where T:class
    {
        // if (request.Content is JsonContent)
        // {
        //     var jsonString = await request.Content.ReadAsStringAsync();
        //     Console.WriteLine($"Request content = {jsonString}");            
        // }

        var responseMessage = await BuildAndExecuteRequest(request);

        var keyValuePairs = responseMessage.Headers
            .Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.FirstOrDefault() ?? ""))
            .ToList();
        // LastModified not part of the responseMessage.Headers; we need to get that separately from the Content.Headers
        if (responseMessage.Content.Headers.LastModified != null)
        {
            keyValuePairs.Add(new KeyValuePair<string, string>("Last-Modified", responseMessage.Content.Headers.LastModified.ToString()!));
        }
        try
        {
            // var jsonString = await responseMessage.Content.ReadAsStringAsync();
            // Console.WriteLine($"Response content = {jsonString}");            
            var responseBody = typeof(T) == typeof(string)
                ? await responseMessage.Content.ReadAsStringAsync() as T
                : await responseMessage.Content.ReadFromJsonAsync<T>()
                               ?? throw new HsdpRequestException("Response body should not be null");
            return new HsdpResponse<T>(keyValuePairs, responseBody!);
        }
        catch (NotSupportedException e)
        {
            throw new HsdpRequestException("Invalid content type", e);
        }
        catch (JsonException e)
        {
            throw new HsdpRequestException("Invalid JSON", e);
        }
    }
    
    private static async Task<HttpResponseMessage> BuildAndExecuteRequest(IHsdpRequest request)
    {
        var uri = ComposeUriFromPathAndQueryParameters(request.Path, request.QueryParameters);
        var requestMessage = new HttpRequestMessage(request.Method, uri);
        foreach (var (key, value) in request.Headers)
        {
            requestMessage.Headers.Add(key, value);
        }
        requestMessage.Content = request.Content;
        
        using var client = new HttpClient();
        var responseMessage = await client.SendAsync(requestMessage);
        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new HsdpRequestException($"Request failed with status {responseMessage.StatusCode}, message {await responseMessage.Content.ReadAsStringAsync()}");
        }
        return responseMessage;
    }

    private static Uri ComposeUriFromPathAndQueryParameters(Uri path, List<KeyValuePair<string, string>> queryParameters)
    {
        var builder = new UriBuilder(path)
        {
            Port = -1
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        foreach (var (key, value) in queryParameters)
        {
            query[key] = value;
        }

        builder.Query = query.ToString();
        var uri = builder.ToString();
        return new Uri(uri);
    }
}