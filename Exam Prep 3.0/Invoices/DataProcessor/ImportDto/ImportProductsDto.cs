using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductsDto
    {
        [Required]
        [MaxLength(Constants.productNameMaxLength)]
        [MinLength(Constants.productNameMinLength)]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [Range((double)Constants.productPriceMinValue, (double)Constants.productPriceMaxValue)]
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [Required]
        [Range(0,4)]
        [JsonProperty("CategoryType")]
        public int CategoryType { get; set; }

        [JsonProperty("Clients")]
        public int[] ClientIds { get; set; }
    }
}
