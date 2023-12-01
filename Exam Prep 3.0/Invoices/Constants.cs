using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices
{
    public class Constants
    {
        //Client
        public const int clientNameMaxLength = 25;
        public const int clientNameMinLength = 10;
        public const int clientNumeberVatMaxLength = 15;
        public const int clientNumeberVatMinLength = 10;

        //Invoice
        public const int invoiceNumberMaxValue = 1500000000;
        public const int invoiceNumberMinValue = 1000000000;

        //Address
        public const int addressStreetNameMaxLength = 20;
        public const int addressStreetNameMinLength = 10;
        public const int addressCityMaxLength = 15;
        public const int addressCityMinLength = 5;
        public const int addressCountryMaxLength = 15;
        public const int addressCountryMinLength = 5;

        //Product
        public const int productNameMaxLength = 30;
        public const int productNameMinLength = 9;
        public const int productPriceMaxValue = 1000;
        public const int productPriceMinValue = 5;
    }
}
