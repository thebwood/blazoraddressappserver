
using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;
using AddressAppServer.ClassLibrary.Mappers;
using AddressAppServer.Web.Services.Interfaces;

namespace AddressAppServer.Web.ViewModels.Admin
{
    public class UsersViewModel
    {
        private readonly IAdminClient _adminClient;

        public UsersViewModel(IAdminClient adminClient)
        {
            _adminClient = adminClient;
        }

        public async Task GetUsers()
        {
            Result<UsersResponseDTO> result = await _adminClient.GetUsers();
            if (result.Success)
            {
                List<UserModel> userModels = result.Value.Users.Select(dto => UserMapper.MapToUserModel(dto)).ToList();
                UsersLoaded(userModels);
            }
        }

        internal async Task DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public Action<List<UserModel>>? UsersLoaded;

    }
}
