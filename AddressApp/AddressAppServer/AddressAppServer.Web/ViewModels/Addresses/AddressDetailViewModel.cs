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
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        }

        public void GetAddress() => OnAddressLoaded?.Invoke(new AddressModel());

        public async Task GetAddress(Guid id)
        {
            var response = await _addressService.GetAddress(id).ConfigureAwait(false);

            if (response?.Value == null)
            {
                OnAddressLoaded?.Invoke(null);
                return;
            }
            OnAddressLoaded?.Invoke(AddressMapper.MapToAddressModel(response.Value.Address));
        }

        internal async Task SaveAddressAsync(AddressModel address)
        {
            var addressDTO = new AddressDTO
            {
                Id = address.Id,
                StreetAddress = address.StreetAddress,
                StreetAddress2 = address.StreetAddress2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode
            };

            Result result;
            if (IsNew)
            {
                AddAddressRequestDTO? addAddressDTO = new AddAddressRequestDTO { Address = addressDTO };
                result = await _addressService.CreateAddress(addAddressDTO);
            }
            else
            {
                UpdateAddressRequestDTO? updateAddressDTO = new UpdateAddressRequestDTO { Address = addressDTO };
                result = await _addressService.UpdateAddress(updateAddressDTO);
            }

            OnAddressSaved?.Invoke(result);
        }

        public Action<AddressModel>? OnAddressLoaded { get; set; }
        public Action<Result>? OnAddressSaved { get; set; }
    }
}
