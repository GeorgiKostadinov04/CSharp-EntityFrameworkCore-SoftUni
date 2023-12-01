using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Address")]
    public class ImportAddressesDto
    {
        [Required]
        [MaxLength(Constants.addressStreetNameMaxLength)]
        [MinLength(Constants.addressStreetNameMinLength)]
        [XmlElement("StreetName")]
        public string StreetName { get; set; }

        [Required]
        [XmlElement("StreetNumber")]
        public int StreetNumber { get; set; }

        [Required]
        [XmlElement("PostCode")]
        public string PostCode { get; set; }

        [Required]
        [MaxLength(Constants.addressCityMaxLength)]
        [MinLength(Constants.addressCityMinLength)]
        [XmlElement("City")]
        public string City { get; set; }

        [Required]
        [MaxLength(Constants.addressCountryMaxLength)]
        [MinLength(Constants.addressCountryMinLength)]
        [XmlElement("Country")]
        public string Country { get; set; }
    }
}