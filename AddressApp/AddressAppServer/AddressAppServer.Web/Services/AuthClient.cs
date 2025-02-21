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

        public async Task<Result<string>> LoginAsync(UserLoginModel loginModel)
        {
            Result<string> result = new Result<string>();
            UserLoginRequestDTO loginRequest = new UserLoginRequestDTO
            {
                UserName = loginModel.Username,
                Password = loginModel.Password
            };
            string jsonPayload = JsonSerializer.Serialize(loginRequest);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync("api/auth/login", requestContent);
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result<string>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            if (result.Success)
            {
                await _protectedSessionStorage.SetAsync("authToken", result.Value);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Value);
                
            }
            return result;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
            await _protectedSessionStorage.DeleteAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
