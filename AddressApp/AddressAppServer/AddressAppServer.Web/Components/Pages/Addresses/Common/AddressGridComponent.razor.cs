using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.Components.Pages.Addresses.Common
{
    public partial class AddressGridComponent : CommonBase, IDisposable
    {
        [Parameter]
        public AddressesViewModel AddressesViewModel { get; set; }

        private List<AddressModel> _addresses { get; set; } = new ();
        protected override async Task OnInitializedAsync()
        {
            AddressesViewModel.AddressesLoaded += OnAddressesLoaded;
        }

        private void OnAddressesLoaded(List<AddressModel> list)
        {
            _addresses = list;
            StateHasChanged();
        }

        public void Dispose()
        {
            AddressesViewModel.AddressesLoaded -= OnAddressesLoaded;
        }

        public void EditAddress(Guid id)
        {
            NavigationManager.NavigateTo($"/addresses/{id}");
        }

        public void DeleteAddress(Guid id)
        {
            // Implement delete functionality here
        }

    }
}
