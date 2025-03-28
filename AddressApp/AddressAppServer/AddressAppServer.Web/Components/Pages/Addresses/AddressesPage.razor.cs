﻿using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.Web.BaseClasses;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AddressAppServer.Web.Components.Pages.Addresses
{
    public partial class AddressesPage : ProtectedPageBase, IDisposable
    {
        [Inject]
        private AddressesViewModel AddressesViewModel { get; set; }
        [Inject]
        private UIStateViewModel _stateViewModel { get; set; }

        [Inject]
        private ILogger<AddressesPage> Logger { get; set; }

        protected override void OnInitialized()
        {
            AddressesViewModel.OnAddressesDeleted += AddressesDeleted;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    _stateViewModel.IsLoading = true;
                    await AddressesViewModel.GetAddresses();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error loading addresses");
                    Snackbar.Add("An error occurred while loading addresses.", Severity.Error);
                }
                finally
                {
                    _stateViewModel.IsLoading = false;
                }
            }
        }

        private void CreateAddress()
        {
            NavigationManager.NavigateTo("/Addresses/Create");
        }

        private async void AddressesDeleted(Result result)
        {
            try
            {
                Snackbar.Add(result.Message, Severity.Success);
                _stateViewModel.IsLoading = true;
                await AddressesViewModel.GetAddresses();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error reloading addresses after deletion");
                Snackbar.Add("An error occurred while reloading addresses.", Severity.Error);
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