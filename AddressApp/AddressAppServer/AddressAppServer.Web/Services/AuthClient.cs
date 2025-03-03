using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using System.Text;
using AddressAppServer.Web.Security;
using System.Net;

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

        public async Task<Result<UserLoginResponseDTO>> LoginAsync(UserLoginModel loginModel)
        {
            Result<UserLoginResponseDTO> result = new();
            UserLoginRequestDTO loginRequest = new UserLoginRequestDTO
            {
                UserName = loginModel.Username,
                Password = loginModel.Password
            };
            string jsonPayload = JsonSerializer.Serialize(loginRequest);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync("api/auth/login", requestContent);
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result<UserLoginResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            if (result.Success)
            {
                await _authStateProvider.MarkUserAsAuthenticated(result.Value.User, result.Value.Token, result.Value.RefreshToken);
            }
            return result;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await _authStateProvider.MarkUserAsLoggedOut();
        }

        public async Task<Result<RefreshUserTokenResponseDTO>> RefreshTokenAsync(string refreshToken)
        {
            var refreshRequest = new RefreshUserTokenRequestDTO
            {
                User = new UserDTO(),
                RefreshToken = refreshToken
            };

            string jsonPayload = JsonSerializer.Serialize(refreshRequest);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync("api/auth/refresh", requestContent);
            string? content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<RefreshUserTokenResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            if (result.Success)
            {
                await _authStateProvider.MarkUserAsAuthenticated(result.Value.User, result.Value.Token, result.Value.RefreshToken);
            }
            else
            {
                await _authStateProvider.MarkUserAsLoggedOut();
            }

            return result;
        }
    }
}
