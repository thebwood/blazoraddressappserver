using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace AddressAppServer.Web.ViewModels.Auth
{
    public class LoginViewModel
    {
        private readonly IAuthClient _authClient;

        public LoginViewModel(IAuthClient authClient)
        {
            _authClient = authClient;
        }

        public async Task<Result> LoginAsync(UserLoginModel loginModel)
        {
            Result? result = await _authClient.LoginAsync(loginModel);

            return result;
        }
    }
}
