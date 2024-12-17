using Digital.Net.Authentication.Controllers.Models;
using Digital.Net.Core.Extensions.HttpUtilities;
using Digital.Net.Core.Messages;
using Newtonsoft.Json;

namespace InternalTestProgram.Extensions;

public static class BaseClientExtensions
{
    private const string ApiUrl = "/authentication/testuser";

    public static async Task<HttpResponseMessage> Login(this HttpClient client, string login, string password)
    {
        var response = await client.PostAsJsonAsync(
            $"{ApiUrl}/login",
            new LoginPayload { Login = login, Password = password }
        );
        await client.SetAuthorizations(response);
        return response;
    }

    public static async Task<HttpResponseMessage> Logout(this HttpClient client)
    {
        var response = await client.PostAsync($"{ApiUrl}/logout", null);
        client.DefaultRequestHeaders.Remove("Authorization");
        return response;
    }

    public static async Task<HttpResponseMessage> LogoutAll(this HttpClient client)
    {
        var response = await client.PostAsync($"{ApiUrl}/logout-all", null);
        client.DefaultRequestHeaders.Remove("Authorization");
        return response;
    }

    public static async Task<HttpResponseMessage> RefreshTokens(this HttpClient client)
    {
        var response = await client.PostAsync($"{ApiUrl}/refresh", null);
        await client.SetAuthorizations(response);
        return response;
    }

    public static async Task SetAuthorizations(
        this HttpClient client,
        HttpResponseMessage loginResponse
    )
    {
        try
        {
            var content = await loginResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Result<string>>(content)!.Value;
            var refreshToken = loginResponse.TryGetCookie();

            if (refreshToken is not null)
                client.AddCookie(refreshToken);
            if (token is not null)
                client.AddAuthorization(token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}