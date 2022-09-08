using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

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
        var responseMessage = await BuildAndExecuteRequest(request);

        var keyValuePairs = responseMessage.Headers
            .Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.FirstOrDefault() ?? ""))
            .ToList();
        try
        {
            var responseBody = await responseMessage.Content.ReadFromJsonAsync<T>()
                               ?? throw new HsdpRequestException("Response body should not be null");
            return new HsdpResponse<T>(keyValuePairs, responseBody);
        }
        catch (NotSupportedException e) // When content type is not valid
        {
            throw new HsdpRequestException("Invalid content type", e);
        }
        catch (JsonException e) // Invalid JSON
        {
            throw new HsdpRequestException("Invalid JSON", e);
        }
    }
    
    private static async Task<HttpResponseMessage> BuildAndExecuteRequest(IHsdpRequest request)
    {
        var requestMessage = new HttpRequestMessage(request.Method, request.Path);
        foreach (var (key, value) in request.Headers)
        {
            requestMessage.Headers.Add(key, value);
        }

        // TODO: handle query parameters
        // if (request.QueryParameters != null)
        // {
        //     foreach (var (key, value) in request.QueryParameters)
        //     {
        //         requestMessage.Options.Set(key, value);
        //     }
        // }
        requestMessage.Content = request.Content;
        
        using var client = new HttpClient();
        var responseMessage = await client.SendAsync(requestMessage);
        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new HsdpRequestException($"Request failed with status {responseMessage.StatusCode}, message {await responseMessage.Content.ReadAsStringAsync()}");
        }
        return responseMessage;
    }
}