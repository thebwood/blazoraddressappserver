using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;

namespace AddressAppServer.Web.Services.Interfaces
{
    public interface IAddressClient
    {
        Task<Result<GetAddressesResponseDTO>> GetAddresses();
        Task<Result<GetAddressResponseDTO>> GetAddress(Guid id);

        Task<Result> AddAddress(AddAddressRequestDTO addressDTO);

        Task<Result> UpdateAddress(UpdateAddressRequestDTO addressDTO);
        Task<Result> DeleteAddress(Guid id);

    }
}
