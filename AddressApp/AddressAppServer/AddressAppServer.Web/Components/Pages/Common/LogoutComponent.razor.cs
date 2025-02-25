using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Auth;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Common
{
    public partial class LogoutComponent : CommonBase
    {
        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }
        [Inject]
        private LogoutViewModel _logoutViewModel { get; set; }

        private async Task Logout()
        {
            try
            {
                _stateViewModel.IsLoading = true;
                await _logoutViewModel.LogoutAsync();
                NavigationManager.NavigateTo("/Login");
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }

        }

    }
}
