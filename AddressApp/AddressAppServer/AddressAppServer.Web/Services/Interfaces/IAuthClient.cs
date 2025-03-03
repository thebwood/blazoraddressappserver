using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;

namespace AddressAppServer.Web.Services.Interfaces
{
    public interface IAuthClient
    {
        Task<Result<UserLoginResponseDTO>> LoginAsync(UserLoginModel loginModel);
        Task LogoutAsync();
        Task<Result<RefreshUserTokenResponseDTO>> RefreshTokenAsync(string refreshToken);
    }
}
