using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Addresses
{
    public partial class UpdateAddressPage : CommonBase
    {
        [Parameter]
        public Guid AddressId { get; set; }
        [Inject]
        private AddressDetailViewModel _addressViewModel { get; set; }
        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _addressViewModel.IsNew = false;
                _stateViewModel.IsLoading = true;
                await _addressViewModel.GetAddress(AddressId);
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }
        }
    }
}