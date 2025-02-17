using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressAppServer.ClassLibrary.Models
{
    public class AddressModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Street Address cannot be longer than 100 characters.")]
        public string StreetAddress { get; set; }

        [StringLength(100, ErrorMessage = "Street Address 2 cannot be longer than 100 characters.")]
        public string StreetAddress2 { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters.")]
        public string City { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "State cannot be longer than 2 characters.")]
        public string State { get; set; }

        [Required]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Postal Code must be exactly 5 digits.")]
        public string PostalCode { get; set; }
    }
}