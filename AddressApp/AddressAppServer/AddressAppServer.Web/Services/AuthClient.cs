using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

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
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                await _protectedSessionStorage.SetAsync("authToken", token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return new Result { Token = token, StatusCode = response.StatusCode };
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<Error>();
                return new Result { Message = error.Name, StatusCode = response.StatusCode, Errors = new List<Error> { error } };
            }
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
            await _protectedSessionStorage.DeleteAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
