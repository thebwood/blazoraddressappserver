using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Admin;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Admin.Common
{
    public partial class UserGridComponent : CommonBase, IDisposable
    {
        [Parameter]
        public UsersViewModel UsersViewModel { get; set; }

        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        private List<UserModel> _users { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            UsersViewModel.UsersLoaded += UsersLoaded;
        }

        private void UsersLoaded(List<UserModel> list)
        {
            _users = list;
            StateHasChanged();
        }

        public void Dispose()
        {
            UsersViewModel.UsersLoaded -= UsersLoaded;
        }

        public void EditUser(Guid id)
        {
            NavigationManager.NavigateTo($"/users/edit/{id}");
        }

        public async Task DeleteUser(Guid id)
        {
            try
            {
                _stateViewModel.IsLoading = true;
                await UsersViewModel.DeleteUser(id);
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }
        }
    }
}
