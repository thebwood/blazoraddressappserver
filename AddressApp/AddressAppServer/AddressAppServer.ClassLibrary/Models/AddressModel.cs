using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressAppServer.ClassLibrary.Models
{
    public class AddressModel
    {
        public Guid Id { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

    }
}
