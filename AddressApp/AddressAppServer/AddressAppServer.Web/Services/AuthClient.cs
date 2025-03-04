using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using System.Text;
using AddressAppServer.Web.Security;
using System.Net;
using Microsoft.AspNetCore.Identity.Data;

namespace AddressAppServer.Web.Services
{
    public class AuthClient : IAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly AddressAuthenticationStateProvider _authStateProvider;
        private readonly IConfiguration _configuration;

        public AuthClient(HttpClient httpClient, AddressAuthenticationStateProvider authStateProver, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProver;
            _configuration = configuration;
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

        public async Task<Result<RefreshUserTokenResponseDTO>> RefreshTokenAsync(UserDTO userDto, string refreshToken)
        {
            RefreshUserTokenRequestDTO refreshRequest = new RefreshUserTokenRequestDTO
            {
                User = userDto,
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
