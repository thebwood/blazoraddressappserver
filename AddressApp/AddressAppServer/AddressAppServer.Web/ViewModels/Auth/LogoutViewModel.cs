using AddressAppServer.Web.Services.Interfaces;

namespace AddressAppServer.Web.ViewModels.Auth
{
    public class LogoutViewModel
    {
        private readonly IAuthClient _authClient;

        public LogoutViewModel(IAuthClient authClient)
        {
            _authClient = authClient;
        }
        public async Task LogoutAsync()
        {
            await _authClient.LogoutAsync();
        }
    }
}
