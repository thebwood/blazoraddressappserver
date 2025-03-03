using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Mappers;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Services.Interfaces;

namespace AddressAppServer.Web.ViewModels.Addresses
{
    public class AddressesViewModel
    {
        private readonly IAddressClient _addressService;

        public AddressesViewModel(IAddressClient addressService)
        {
            _addressService = addressService;
        }

        public async Task GetAddresses()
        {
            Result<GetAddressesResponseDTO> result = await _addressService.GetAddresses();
            if (result.Success)
            {
                List<AddressModel> addressViewModels = result.Value.AddressList.Select(dto => AddressMapper.MapToAddressModel(dto)).ToList();
                AddressesLoaded(addressViewModels);
            }
            else
            {
                // Handle errors (e.g., log the error, show a message to the user, etc.)
                Console.WriteLine($"Error fetching addresses: {result.Message}");
            }
        }

        public async Task DeleteAddress(Guid id)
        {
            Result result = await _addressService.DeleteAddress(id);
            if (result.Success)
            {
                AddressesDeleted(result);
            }
        }

        private void AddressesLoaded(List<AddressModel> addresses)
        {
            OnAddressesLoaded?.Invoke(addresses);
        }

        private void AddressesDeleted(Result result)
        {
            OnAddressesDeleted?.Invoke(result);
        }


        public Action<List<AddressModel>>? OnAddressesLoaded;
        public Action<Result>? OnAddressesDeleted;

    }
}
