using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotnetHsdpSdk.Utils;

public interface IHttp
{
    Task<string> HttpSendRequest(HttpRequestMessage request);
    Task<T> HttpSendRequest<T>(HttpRequestMessage request);
}

internal class Http : IHttp
{
    public async Task<string> HttpSendRequest(HttpRequestMessage request)
    {
        using var client = new HttpClient();
        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Request failed: {responseBody}");
            throw new AuthenticationException("HTTP request failed with status code: " + response.StatusCode);
        }

        return responseBody;
    }

    public async Task<T> HttpSendRequest<T>(HttpRequestMessage request)
    {
        var json = await HttpSendRequest(request);
        try
        {
            var tokenResponse = JsonSerializer.Deserialize<T>(json);
            if (tokenResponse == null) throw new AuthenticationException("Unable to parse TokenResponse from json.");
            return tokenResponse;
        }
        catch (JsonException e)
        {
            throw new AuthenticationException("Unable to parse TokenResponse from JSON.", e);
        }
    }
}