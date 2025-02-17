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

        public async Task<Result> AddAddress(AddAddressRequestDTO requestDTO)
        {
            StringContent content = new StringContent(JsonSerializer.Serialize(requestDTO), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PostAsync("api/addresses", content);

            return new Result
            {
                StatusCode = response.StatusCode,
                Errors = response.IsSuccessStatusCode ? new List<Error> { Error.None } : new List<Error> { new Error("Error.AddAddress", response.ReasonPhrase) }
            };
        }

        public async Task<Result> UpdateAddress(UpdateAddressRequestDTO addressDTO)
        {
            StringContent content = new StringContent(JsonSerializer.Serialize(addressDTO), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PutAsync("api/addresses", content);

            return new Result
            {
                StatusCode = response.StatusCode,
                Errors = response.IsSuccessStatusCode ? new List<Error> { Error.None } : new List<Error> { new Error("Error.UpdateAddress", response.ReasonPhrase) }
            };
        }

        public async Task<Result> DeleteAddress(Guid id)
        {
            using HttpResponseMessage response = await _httpClient.DeleteAsync($"api/addresses/{id}");

            return new Result
            {
                StatusCode = response.StatusCode,
                Errors = response.IsSuccessStatusCode ? new List<Error> { Error.None } : new List<Error> { new Error("Error.DeleteAddress", response.ReasonPhrase) }
            };
        }
    }
}
