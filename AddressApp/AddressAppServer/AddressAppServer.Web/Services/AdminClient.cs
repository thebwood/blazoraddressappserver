using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Security;
using AddressAppServer.Web.Services.Interfaces;
using System.Text.Json;

namespace AddressAppServer.Web.Services
{
    public class AdminClient : IAdminClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminClient> _logger;
        private readonly AddressAuthenticationStateProvider _authStateProvider;

        public AdminClient(HttpClient httpClient, ILogger<AdminClient> logger, AddressAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _logger = logger;
            _authStateProvider = authStateProvider;
        }

        public async Task<Result<UsersResponseDTO>> GetUsers()
        {
            Result<UsersResponseDTO> result = new();
            if (await _authStateProvider.SetAuthorizationHeaderAsync(_httpClient))
            {
                var response = await _httpClient.GetAsync("api/Admin/Users");
                string? content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Result<UsersResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                result.Message = "Authentication token not found.";
            }
            return result;
        }
    }
}
