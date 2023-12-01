using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoicesDto
    {
        [Required]
        [Range(Constants.invoiceNumberMinValue, Constants.invoiceNumberMaxValue)]
        [JsonProperty("Number")]
        public int Number {  get; set; }

        [Required]
        [JsonProperty("IssueDate")]
        public DateTime IssueDate { get; set; }

        [Required]
        [JsonProperty("DueDate")]
        public DateTime DueDate { get; set; }

        [Required]
        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Range(0,2)]
        [JsonProperty("CurrencyType")]
        public int CurrencyType { get; set; }

        [Required]
        [JsonProperty("ClientId")]
        public int ClientId { get; set; }
    }
}
