using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace AddressAppServer.Web.Services
{
    public class StorageService : IStorageService
    {
        private readonly ProtectedLocalStorage _localStorage;

        public StorageService(ProtectedLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetAccessTokenAsync(string token)
        {
            await _localStorage.SetAsync("accessToken", token);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var tokenResult = await _localStorage.GetAsync<string>("accessToken");
            return tokenResult.Success ? tokenResult.Value : null;
        }

        public async Task SetRefreshTokenAsync(string refreshToken)
        {
            await _localStorage.SetAsync("refreshToken", refreshToken);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            var tokenResult = await _localStorage.GetAsync<string>("refreshToken");
            return tokenResult.Success ? tokenResult.Value : null;
        }

        public async Task SetUserAsync(UserDTO user)
        {
            await _localStorage.SetAsync("user", user);
        }

        public async Task<UserDTO?> GetUserAsync()
        {
            var userResult = await _localStorage.GetAsync<UserDTO>("user");
            return userResult.Success ? userResult.Value : null;
        }

        public async Task ClearStorageAsync()
        {
            await _localStorage.DeleteAsync("accessToken");
            await _localStorage.DeleteAsync("refreshToken");
            await _localStorage.DeleteAsync("user");
        }
    }
}
