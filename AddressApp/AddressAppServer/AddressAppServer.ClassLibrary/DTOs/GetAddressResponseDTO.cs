using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressAppServer.ClassLibrary.DTOs
{
    public class GetAddressResponseDTO
    {
        public GetAddressResponseDTO()
        {
            Address = new AddressDTO();
        }

        public AddressDTO Address { get; set; }

    }
}
