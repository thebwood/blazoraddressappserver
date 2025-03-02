namespace AddressAppServer.ClassLibrary.DTOs
{
    public class RefreshUserTokenRequestDTO
    {
        public UserDTO User { get; set; }
        public string RefreshToken { get; set; }
    }
}
