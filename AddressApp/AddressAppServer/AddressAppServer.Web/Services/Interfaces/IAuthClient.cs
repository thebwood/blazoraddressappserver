using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.Models;

namespace AddressAppServer.Web.Services.Interfaces
{
    public interface IAuthClient
    {
        Task<Result<string>> LoginAsync(UserLoginModel loginModel);
        Task LogoutAsync();
    }
}
