using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using System.Text;
using AddressAppServer.Web.Security;

namespace AddressAppServer.Web.Services
{
    public class AuthClient : IAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly JWTAuthenticationStateProvider _authStateProvider;
        private readonly IConfiguration _configuration;
        private readonly ProtectedSessionStorage _protectedSessionStorage;

        public AuthClient(HttpClient httpClient, JWTAuthenticationStateProvider authStateProver, IConfiguration configuration, ProtectedSessionStorage protectedSessionStorage)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProver;
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
                await _authStateProvider.MarkUserAsAuthenticated(result.Token);

            }
            return result;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await _authStateProvider.MarkUserAsLoggedOut();
        }
    }
}
