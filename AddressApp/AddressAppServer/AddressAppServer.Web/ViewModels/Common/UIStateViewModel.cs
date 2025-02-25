using AddressAppServer.Web.Services.Interfaces;
using System.ComponentModel;

namespace AddressAppServer.Web.ViewModels.Common
{
    public class UIStateViewModel : INotifyPropertyChanged
    {

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
