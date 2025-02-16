using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Mappers;
using AddressAppServer.Web.Services.Interfaces;

namespace AddressAppServer.Web.ViewModels.Addresses
{
    public class AddressDetailViewModel
    {
        private readonly IAddressClient _addressService;

        public AddressDetailViewModel(IAddressClient addressService)
        {
            _addressService = addressService;
        }

        public async Task GetAddress(Guid id)
        {
            Result<GetAddressResponseDTO>? response = await _addressService.GetAddress(id);

            if (response == null || response.Value == null)
            {
                AddressLoaded?.Invoke(null);
                return;
            }
            AddressLoaded?.Invoke(AddressMapper.MapToAddressModel(response.Value.Address));
        }

        public async Task CreateAddress(AddressModel address)
        {
            AddAddressRequestDTO addressDTO = new AddAddressRequestDTO
            {
                Address = new AddressDTO
                {
                    Id = address.Id,
                    StreetAddress = address.StreetAddress,
                    StreetAddress2 = address.StreetAddress2,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode
                }
            };
            var result = await _addressService.AddAddress(addressDTO);
            AddressCreated?.Invoke(result.Success);
        }

        public async Task UpdateAddress(AddressModel address)
        {
            UpdateAddressRequestDTO? addressDTO = new UpdateAddressRequestDTO
            {
                Address = new AddressDTO
                {
                    Id = address.Id,
                    StreetAddress = address.StreetAddress,
                    StreetAddress2 = address.StreetAddress2,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode
                }
            };
            var result = await _addressService.UpdateAddress(addressDTO);
            AddressUpdated?.Invoke(result.Success);
        }

        public Action<AddressModel>? AddressLoaded { get; set; }
        public Action<bool>? AddressCreated { get; set; }
        public Action<bool>? AddressUpdated { get; set; }
    }
}
