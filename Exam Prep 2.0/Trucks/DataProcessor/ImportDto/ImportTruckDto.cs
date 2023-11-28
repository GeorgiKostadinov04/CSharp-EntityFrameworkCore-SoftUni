using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Trucks.Data.Models.Enums;
using Trucks.Utilities;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTruckDto
    {
        [Required]
        [RegularExpression(@"[A-Z]{2}[0-9]{4}[A-Z]{2}$")]
        public string RegistrationNumber { get; set; }

        [Required]
        [StringLength(GlobalConstants.truckVinNumberLength)]
        public string VinNumber { get; set; }

        [XmlElement("TankCapacity")]
        [Range(950, 1420)]
        public int TankCapacity { get; set; }

        [XmlElement("CargoCapacity")]
        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        [Required]
        [XmlElement("CategoryType")]
        [Range(0, 3)]
        public int CategoryType { get; set; }

        [Required]
        [XmlElement("MakeType")]
        [Range(0, 4)]
        public int MakeType { get; set; }
    }
}