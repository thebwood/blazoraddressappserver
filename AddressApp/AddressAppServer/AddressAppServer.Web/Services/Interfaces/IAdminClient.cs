using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;

namespace AddressAppServer.Web.Services.Interfaces
{
    public interface IAdminClient
    {
        Task<Result<UsersResponseDTO>> GetUsers();
    }
}
