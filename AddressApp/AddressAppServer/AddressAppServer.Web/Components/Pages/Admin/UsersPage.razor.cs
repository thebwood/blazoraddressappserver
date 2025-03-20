using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.Components.Pages.Addresses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Admin;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Admin
{
    public partial class UsersPage : ProtectedPageBase, IDisposable
    {
        [Inject]
        private UsersViewModel UsersViewModel { get; set; }

        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        private List<UserModel> _users = new();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            UsersViewModel.UsersLoaded += UsersLoaded;
        }

        public void Dispose()
        {
            UsersViewModel.UsersLoaded -= UsersLoaded;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    _stateViewModel.IsLoading = true;
                    await UsersViewModel.GetUsers();
                }
                finally
                {
                    _stateViewModel.IsLoading = false;
                }
            }
        }

        private void UsersLoaded(List<UserModel> list)
        {
            _users = list;
            StateHasChanged();
        }
    }
}
