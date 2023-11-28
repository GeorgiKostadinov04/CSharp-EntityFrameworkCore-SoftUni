using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trucks.Utilities;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class ImportDispatcherDto
    {
        [Required]
        [MaxLength(GlobalConstants.despatherNameMaxLength)]
        [MinLength(GlobalConstants.despatcherNameMinLength)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Position")]
        public string Position { get; set; }

        [XmlArray("Trucks")]
        public virtual ImportTruckDto[] Trucks { get; set; }
    }
}
