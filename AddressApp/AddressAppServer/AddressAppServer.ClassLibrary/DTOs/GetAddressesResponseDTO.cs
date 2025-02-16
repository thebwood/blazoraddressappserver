namespace AddressAppServer.ClassLibrary.DTOs
{
    public class GetAddressesResponseDTO
    {
        public GetAddressesResponseDTO()
        {
            AddressList = new List<AddressDTO>();
        }

        public List<AddressDTO> AddressList { get; set; }

    }
}
