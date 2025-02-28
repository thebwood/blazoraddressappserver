using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Addresses
{
    public partial class CreateAddressPage : ProtectedPageBase
    {
        [Inject]
        private AddressDetailViewModel _addressViewModel { get; set; }
        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            try
            {
                _addressViewModel.IsNew = true;
                _stateViewModel.IsLoading = true;
                _addressViewModel.GetAddress();
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }
        }
    }
}