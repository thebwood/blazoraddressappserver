using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;

namespace AddressAppServer.ClassLibrary.Mappers
{
    public static class AddressMapper
    {
        public static AddressModel MapToAddressModel(AddressDTO dto)
        {
            return new AddressModel
            {
                Id = dto.Id ?? Guid.Empty, // Handle nullable Guid
                StreetAddress = dto.StreetAddress,
                StreetAddress2 = dto.StreetAddress2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode
            };
        }

        public static AddressDTO MapToAddressDTO(AddressModel model)
        {
            return new AddressDTO
            {
                Id = model.Id,
                StreetAddress = model.StreetAddress,
                StreetAddress2 = model.StreetAddress2,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode
            };
        }
    }
}
