using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AddressAppServer.Web.Components.Pages.Addresses
{
    public partial class AddressesPage : CommonBase, IDisposable
    {
        [Inject]
        private AddressesViewModel AddressesViewModel { get; set; }
        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AddressesViewModel.OnAddressesDeleted += AddressesDeleted;

            try
            {
                _stateViewModel.IsLoading = true;
                await AddressesViewModel.GetAddresses();
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }
        }

        private void CreateAddress()
        {
            NavigationManager.NavigateTo($"/Addresses/Create");
        }
        private void AddressesDeleted(Result result)
        {
            try
            {
                Snackbar.Add(result.Message, Severity.Success);
                _stateViewModel.IsLoading = true;
                AddressesViewModel.GetAddresses();
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }
        }

        public void Dispose()
        {
            AddressesViewModel.OnAddressesDeleted -= AddressesDeleted;
        }
    }
}