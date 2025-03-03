using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.ClassLibrary.Models;

namespace AddressAppServer.ClassLibrary.Mappers
{
    public static class UserMapper
    {
        public static UserModel MapToUserModel(UserDTO userDTO)
        {
            return new UserModel
            {
                Id = userDTO.Id,
                Username = userDTO.Username,
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName
            };
        }
        public static UserDTO MapToUserDTO(UserModel userModel)
        {
            return new UserDTO
            {
                Id = userModel.Id,
                Username = userModel.Username,
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName
            };
        }
    }
}
