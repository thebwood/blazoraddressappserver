using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel;

namespace AddressAppServer.Web.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject]
        private UIStateViewModel _viewModel { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private bool _drawerOpen = false;
        private bool isAuthenticated;

        protected override async Task OnInitializedAsync()
        {
            _viewModel.PropertyChanged += OnPropertyChanged;

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            isAuthenticated = authState.User.Identity.IsAuthenticated;
        }

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

        public void Dispose()
        {
            _viewModel.PropertyChanged -= OnPropertyChanged;
        }

        private void Login()
        {
            NavigationManager.NavigateTo("login");
        }

        private void Logout()
        {
            NavigationManager.NavigateTo("logout");
        }
    }
}
