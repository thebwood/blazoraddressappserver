using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace AddressAppServer.Web.Components.Pages.Addresses.Common
{
    public partial class AddressDetailComponent : CommonBase, IDisposable
    {
        [Parameter]
        public AddressDetailViewModel AddressesViewModel { get; set; }

        private AddressModel _address { get; set; } = new();
        private EditContext _editContext;

        protected override void OnInitialized()
        {
            _editContext = new(_address);
            AddressesViewModel.OnAddressLoaded += AddressLoaded;
            AddressesViewModel.OnAddressSaved += AddressSaved;
        }
        private void BackToAddresses()
        {
            NavigationManager.NavigateTo($"/Addresses");
        }

        private void AddressSaved(Result result)
        {
            if(result.Success)
            {
                Snackbar.Add("Address Saved Successfully");
                NavigationManager.NavigateTo($"/Addresses");
            }
        }

        private void AddressLoaded(AddressModel address)
        {
            _address = address;
            StateHasChanged();
        }

        private async Task SaveAddress()
        {
            if(_editContext.Validate())
            {
                await AddressesViewModel.SaveAddressAsync(_address);
            }
            else
            {
                Snackbar.Add("Please fix the validation errors", Severity.Error);
            }
        }



        public void Dispose()
        {
            AddressesViewModel.OnAddressLoaded -= AddressLoaded;
            AddressesViewModel.OnAddressSaved -= AddressSaved;

        }


    }
}
