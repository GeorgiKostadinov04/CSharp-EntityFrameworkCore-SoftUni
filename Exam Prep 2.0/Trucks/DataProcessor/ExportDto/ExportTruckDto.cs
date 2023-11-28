using System.Xml.Serialization;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Truck")]
    public class ExportTruckDto
    {
        [XmlElement("RegistrationNumber")]
        public string Number { get; set; }

        [XmlElement("Make")]
        public string Make { get; set; }
    }
}