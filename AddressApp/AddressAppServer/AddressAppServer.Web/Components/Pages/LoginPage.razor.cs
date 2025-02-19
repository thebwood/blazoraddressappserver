using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AddressAppServer.Web.Components.Pages
{
    public partial class LoginPage : CommonBase
    {
        private UserLoginModel loginModel = new UserLoginModel();

        [Inject]
        private IAuthClient AuthClient { get; set; }

        private async Task HandleLogin()
        {
            var result = await AuthClient.LoginAsync(loginModel);

            if (result.Success)
            {
                // If successful, navigate to the home page
                NavigationManager.NavigateTo("/");
            }
            else
            {
                // Show an error message
                Snackbar.Add("Login failed. Please check your credentials and try again.", Severity.Error);
            }
        }
    }

}

