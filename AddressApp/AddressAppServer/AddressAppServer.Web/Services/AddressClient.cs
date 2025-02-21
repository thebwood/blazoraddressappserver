using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text;
using System.Text.Json;

namespace AddressAppServer.Web.Services
{
    public class AddressClient : IAddressClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AddressClient> _logger;
        private readonly ProtectedSessionStorage _protectedSessionStorage;

        public AddressClient(HttpClient httpClient, ILogger<AddressClient> logger, ProtectedSessionStorage protectedSessionStorage)
        {
            _httpClient = httpClient;
            _logger = logger;
            _protectedSessionStorage = protectedSessionStorage;
        }

        private async Task<bool> SetAuthorizationHeaderAsync()
        {
            var tokenResult = await _protectedSessionStorage.GetAsync<string>("authToken");
            if (tokenResult.Success && tokenResult.Value != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResult.Value);
                return true;
            }
            return false;
        }

        public async Task<Result<GetAddressesResponseDTO>> GetAddresses()
        {
            Result<GetAddressesResponseDTO> result = new();
            if (await SetAuthorizationHeaderAsync())
            {
                var response = await _httpClient.GetAsync("api/Addresses");
                string? content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Result<GetAddressesResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                result.Message = "Authentication token not found.";
            }
            return result;
        }

        public async Task<Result<GetAddressResponseDTO>> GetAddress(Guid id)
        {
            Result<GetAddressResponseDTO> result = new();
            if (await SetAuthorizationHeaderAsync())
            {
                var response = await _httpClient.GetAsync($"api/addresses/{id}");
                string? content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Result<GetAddressResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                result.Message = "Authentication token not found.";
            }
            return result;
        }

        public async Task<Result> CreateAddress(AddAddressRequestDTO requestDTO)
        {
            Result result = new();
            string jsonPayload = JsonSerializer.Serialize(requestDTO);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            if (await SetAuthorizationHeaderAsync())
            {
                using HttpResponseMessage response = await _httpClient.PostAsync("api/addresses", requestContent);
                string? content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                result.Message = "Authentication token not found.";
            }
            return result;
        }

        public async Task<Result> UpdateAddress(UpdateAddressRequestDTO requestDTO)
        {
            Result result = new();
            string jsonPayload = JsonSerializer.Serialize(requestDTO);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            if (await SetAuthorizationHeaderAsync())
            {
                using HttpResponseMessage response = await _httpClient.PutAsync("api/addresses", requestContent);
                string? content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                result.Message = "Authentication token not found.";
            }
            return result;
        }

        public async Task<Result> DeleteAddress(Guid id)
        {
            Result result = new();
            if (await SetAuthorizationHeaderAsync())
            {
                var response = await _httpClient.DeleteAsync($"api/addresses/{id}");
                string? content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                result.Message = "Authentication token not found.";
            }
            return result;
        }
    }
}
