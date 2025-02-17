using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Addresses
{
    public partial class AddressesPage : CommonBase
    {
        [Inject]
        private AddressesViewModel AddressesViewModel { get; set; }
        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
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

    }
}