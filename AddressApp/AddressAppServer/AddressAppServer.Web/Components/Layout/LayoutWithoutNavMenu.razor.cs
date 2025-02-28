using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace AddressAppServer.Web.Components.Layout
{
    public partial class LayoutWithoutNavMenu : LayoutComponentBase, IDisposable
    {
        [Inject]
        private UIStateViewModel _viewModel { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        protected override void OnInitialized()
        {
            _viewModel.PropertyChanged += OnPropertyChanged;
        }

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        private void GoToHome()
        {
            NavigationManager.NavigateTo("/");
        }
        private void GoToLogin()
        {
            NavigationManager.NavigateTo("/login");
        }
        public void Dispose()
        {
            _viewModel.PropertyChanged -= OnPropertyChanged;
        }
    }
}
