using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportClientsDto
    {
        [Required]
        [MaxLength(Constants.clientNameMaxLength)]
        [MinLength(Constants.clientNameMinLength)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(Constants.clientNumeberVatMaxLength)]
        [MinLength(Constants.clientNumeberVatMinLength)]
        [XmlElement("NumberVat")]
        public string NumberVat { get; set; }

        [XmlArray("Addresses")]
        public ImportAddressesDto[] Addresses { get; set; }
    }
}
