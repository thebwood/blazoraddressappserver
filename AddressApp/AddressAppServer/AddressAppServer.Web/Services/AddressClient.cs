using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace AddressAppServer.Web.Services
{
    public class AddressClient : IAddressClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AddressClient> _logger;

        public AddressClient(HttpClient httpClient, ILogger<AddressClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result<GetAddressesResponseDTO>> GetAddresses()
        {
            Result<GetAddressesResponseDTO> result = new();

            var response = await _httpClient.GetAsync("api/Addresses");
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result<GetAddressesResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return result;
        }

        public async Task<Result<GetAddressResponseDTO>> GetAddress(Guid id)
        {
            Result<GetAddressResponseDTO> result = new();

            var response = await _httpClient.GetAsync($"api/addresses/{id}");
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result<GetAddressResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return result;
        }

        public async Task<Result> CreateAddress(AddAddressRequestDTO requestDTO)
        {
            Result result = new();
            string jsonPayload = JsonSerializer.Serialize(requestDTO);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync("api/addresses", requestContent);
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return result;
        }

        public async Task<Result> UpdateAddress(UpdateAddressRequestDTO requestDTO)
        {
            Result result = new();
            string jsonPayload = JsonSerializer.Serialize(requestDTO);
            StringContent? requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PutAsync("api/addresses", requestContent);
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return result;
        }

        public async Task<Result> DeleteAddress(Guid id)
        {
            Result result = new();

            var response = await _httpClient.DeleteAsync($"api/addresses/{id}");
            string? content = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            return result;
        }
    }
}
