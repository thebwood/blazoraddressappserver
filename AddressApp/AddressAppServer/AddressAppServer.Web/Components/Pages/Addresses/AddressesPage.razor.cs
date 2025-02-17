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
        private UIStateViewModel _viewModel { get; set; } = default!;

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _viewModel.IsLoading = true;
                await AddressesViewModel.GetAddresses();
            }
            finally
            {
                _viewModel.IsLoading = false;
            }
        }
    }
}