using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using System.Text;

namespace AddressAppServer.Web.Services
{
    public class AuthClient : IAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ProtectedSessionStorage _protectedSessionStorage;

        public AuthClient(HttpClient httpClient, IConfiguration configuration, ProtectedSessionStorage protectedSessionStorage)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _protectedSessionStorage = protectedSessionStorage;
        }

        public async Task<Result> LoginAsync(UserLoginModel loginModel)
        {
            Result result = new ();
            UserLoginRequestDTO loginRequest = new UserLoginRequestDTO
            {
                UserName = loginModel.Username,
                Password = loginModel.Password
            };
            string jsonPayload = JsonSerializer.Serialize(loginRequest);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync("api/auth/login", requestContent);
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            if (result.Success)
            {
                await _protectedSessionStorage.SetAsync("authToken", result.Token);
                await _protectedSessionStorage.SetAsync("refreshToken", result.RefreshToken);
            }
            return result;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
            await _protectedSessionStorage.DeleteAsync("authToken");
            await _protectedSessionStorage.DeleteAsync("refreshToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
