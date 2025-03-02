using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;

namespace AddressAppServer.Web.Services.Interfaces
{
    public interface IAuthClient
    {
        Task<Result<UserLoginResponseDTO>> LoginAsync(UserLoginModel loginModel);
        Task LogoutAsync();
<<<<<<< HEAD
        Task<Result<RefreshUserTokenResponseDTO>> RefreshTokenAsync(UserDTO user, string refreshToken);
=======
>>>>>>> parent of 24950a0 (Working on the refresh token process)
    }
}
