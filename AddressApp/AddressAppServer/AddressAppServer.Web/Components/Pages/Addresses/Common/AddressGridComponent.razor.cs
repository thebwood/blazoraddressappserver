using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Addresses.Common
{
    public partial class AddressGridComponent : CommonBase, IDisposable
    {
        [Parameter]
        public AddressesViewModel AddressesViewModel { get; set; }

        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        private List<AddressModel> _addresses { get; set; } = new ();
        protected override async Task OnInitializedAsync()
        {
            AddressesViewModel.OnAddressesLoaded += AddressesLoaded;
        }

        private void AddressesLoaded(List<AddressModel> list)
        {
            _addresses = list;
            StateHasChanged();
        }

        public void Dispose()
        {
            AddressesViewModel.OnAddressesLoaded -= AddressesLoaded;
        }


        public void EditAddress(Guid id)
        {
            NavigationManager.NavigateTo($"/addresses/{id}");
        }

        public async Task DeleteAddress(Guid id)
        {
            try
            {
                _stateViewModel.IsLoading = true;
                await AddressesViewModel.DeleteAddress(id);
            }
            finally
            {
                _stateViewModel.IsLoading = false;
            }
        }

    }
}
