using AddressAppServer.ClassLibrary.DTOs;

namespace AddressAppServer.Web.Services.Interfaces
{
    public interface IStorageService
    {
        Task SetAccessTokenAsync(string token);
        Task<string?> GetAccessTokenAsync();
        Task SetRefreshTokenAsync(string refreshToken);
        Task<string?> GetRefreshTokenAsync();
        Task SetUserAsync(UserDTO user);
        Task<UserDTO?> GetUserAsync();
        Task ClearStorageAsync();
    }
}
