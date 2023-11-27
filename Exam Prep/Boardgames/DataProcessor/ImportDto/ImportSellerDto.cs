using Boardgames.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerDto
    {
        [Required]
        [MaxLength(Values.sellerNameMaxValue)]
        [MinLength(Values.sellerNameMinValue)]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(Values.sellerAddressMaxValue)]
        [MinLength(Values.sellerAddressMinValue)]
        [JsonProperty("Address")]
        public string Address { get; set; }

        [Required]
        [JsonProperty("Country")]
        public string Country { get; set; }

        [Required]
        [RegularExpression(@"www\.[a-zA-Z0-9\-]{2,256}\.com")]
        [JsonProperty("Website")]
        public string Website { get; set; }

        public int[] Boardgames { get; set; }
    }
}
