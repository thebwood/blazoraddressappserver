namespace AddressAppServer.ClassLibrary.DTOs
{
    public class RefreshUserTokenResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }
}
