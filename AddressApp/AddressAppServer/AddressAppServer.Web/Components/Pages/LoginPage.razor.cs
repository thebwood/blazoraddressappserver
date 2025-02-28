using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Auth;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace AddressAppServer.Web.Components.Pages
{
    public partial class LoginPage : CommonBase
    {
        private UserLoginModel loginModel = new UserLoginModel();
        private EditContext _editContext;

        [Inject]
        private LoginViewModel _loginViewModel { get; set; }

        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }


        protected override void OnInitialized()
        {
            _editContext = new(loginModel);
        }

        private async Task HandleLogin()
        {
            Result? result;
            try
            {
                _stateViewModel.IsLoading = true;
                result = await _loginViewModel.LoginAsync(loginModel);

                if (result.Success)
                {
                    Snackbar.Add("Login successful.", Severity.Success);
                    // If successful, navigate to the dashboard
                    NavigationManager.NavigateTo("/dashboard");
                }
                else
                {
                    // Show an error message
                    Snackbar.Add("Login failed. Please check your credentials and try again.", Severity.Error);
                }
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }
        }
    }

}

