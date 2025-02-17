using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Addresses
{
    public partial class AddressesPage : CommonBase
    {
        [Inject]
        private AddressesViewModel AddressesViewModel { get; set; }

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AddressesViewModel.GetAddresses();
        }
    }
}