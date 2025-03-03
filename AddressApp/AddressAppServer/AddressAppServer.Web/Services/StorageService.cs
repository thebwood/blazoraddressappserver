using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace AddressAppServer.Web.Services
{
    public class StorageService : IStorageService
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public StorageService(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task SetAccessTokenAsync(string token)
        {
            await _sessionStorage.SetAsync("accessToken", token);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("accessToken");
            return tokenResult.Success ? tokenResult.Value : null;
        }

        public async Task SetRefreshTokenAsync(string refreshToken)
        {
            await _sessionStorage.SetAsync("refreshToken", refreshToken);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("refreshToken");
            return tokenResult.Success ? tokenResult.Value : null;
        }

        public async Task SetUserAsync(UserDTO user)
        {
            await _sessionStorage.SetAsync("user", user);
        }

        public async Task<UserDTO?> GetUserAsync()
        {
            var userResult = await _sessionStorage.GetAsync<UserDTO>("user");
            return userResult.Success ? userResult.Value : null;
        }

        public async Task ClearStorageAsync()
        {
            await _sessionStorage.DeleteAsync("accessToken");
            await _sessionStorage.DeleteAsync("refreshToken");
            await _sessionStorage.DeleteAsync("user");
        }
    }
}
