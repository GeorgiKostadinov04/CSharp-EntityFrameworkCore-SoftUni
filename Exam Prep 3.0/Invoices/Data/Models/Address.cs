using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoices.Data.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StreetName { get; set; }

        [Required]
        public int StreetNumber { get; set; }

        [Required]
        public string PostCode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        public Client Client { get; set; }
    }
}