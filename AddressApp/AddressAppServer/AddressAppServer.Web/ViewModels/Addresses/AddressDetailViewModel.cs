using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.Web.Mappers;
using AddressAppServer.Web.Services.Interfaces;

namespace AddressAppServer.Web.ViewModels.Addresses
{
    public class AddressDetailViewModel
    {
        public bool IsNew { get; internal set; }

        private readonly IAddressClient _addressService;

        public AddressDetailViewModel(IAddressClient addressService)
        {
            _addressService = addressService;
        }
        public void GetAddress()
        {
            OnAddressLoaded?.Invoke(new AddressModel());
        }

        
        public async Task GetAddress(Guid id)
        {
            Result<GetAddressResponseDTO>? response = await _addressService.GetAddress(id);

            if (response == null || response.Value == null)
            {
                OnAddressLoaded?.Invoke(null);
                return;
            }
            OnAddressLoaded?.Invoke(AddressMapper.MapToAddressModel(response.Value.Address));
        }

        internal async Task SaveAddressAsync(AddressModel address)
        {
            if(IsNew)
            {
                AddAddressRequestDTO addressDTO = new AddAddressRequestDTO
                {
                };

                var result = await _addressService.AddAddress(addressDTO);
                OnAddressSaved?.Invoke(result);

            }
            else
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
                OnAddressSaved?.Invoke(result);

            }

        }

        public Action<AddressModel>? OnAddressLoaded { get; set; }
        public Action<Result>? OnAddressSaved { get; set; }
    }
}
