using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trucks.Utilities;

namespace Trucks.DataProcessor.ImportDto
{
    public class ImportClientDto
    {
        [Required]
        [MaxLength(GlobalConstants.clientNameMaxLength)]
        [MinLength(GlobalConstants.clientNameMinLength)]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(GlobalConstants.clientNationalityMaxLength)]
        [MinLength (GlobalConstants.clientNationalityMinLength)]
        [JsonProperty("Nationality")]
        public string Nationality { get; set; }

        [Required]
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Trucks")]
        public int[] Trucks {  get; set; } 
    }
}
