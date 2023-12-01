namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Xml.Serialization;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportClientsDto[]), new XmlRootAttribute("Clients"));

            using StringReader xmlReader = new StringReader(xmlString);

            ImportClientsDto[] importClientsDtos = (ImportClientsDto[])xmlSerializer.Deserialize(xmlReader);

            List<Client> clientsToAdd = new List<Client>();

            foreach(ImportClientsDto c in importClientsDtos)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Client client = new Client()
                {
                    Name = c.Name,
                    NumberVat = c.NumberVat
                };

                foreach(ImportAddressesDto a in c.Addresses)
                {
                    if (!IsValid(a))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address address = new Address()
                    {
                        StreetName = a.StreetName,
                        StreetNumber = a.StreetNumber,
                        PostCode = a.PostCode,
                        City = a.City,
                        Country = a.Country,
                    };
                    client.Addresses.Add(address);
                    
                }
                clientsToAdd.Add(client);
                sb.AppendLine(String.Format("Successfully imported client {0}.", client.Name));
            }
            context.Clients.AddRange(clientsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ImportInvoicesDto[] dto = JsonConvert.DeserializeObject<ImportInvoicesDto[]>(jsonString);

            List<Invoice> invoicesToAdd = new List<Invoice>();

            foreach(ImportInvoicesDto i in dto)
            {
                if (!IsValid(i))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (i.DueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture) || i.IssueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Invoice invoice = new Invoice()
                {
                    Number = i.Number,
                    IssueDate = i.IssueDate,
                    DueDate = i.DueDate,
                    Amount = i.Amount,
                    CurrencyType = (CurrencyType)i.CurrencyType,
                    ClientId = i.ClientId
                };

                if (i.IssueDate > i.DueDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                invoicesToAdd.Add(invoice);
                sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoice.Number));
            }
            context.Invoices.AddRange(invoicesToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportProductsDto[] importProductsDtos = JsonConvert.DeserializeObject<ImportProductsDto[]>(jsonString);

            List<Product> products = new List<Product>();

            foreach(ImportProductsDto p in importProductsDtos)
            {
                if (!IsValid(p))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Product product = new Product()
                {
                    Name = p.Name,
                    Price = p.Price,
                    CategoryType = (CategoryType)p.CategoryType
                };

                foreach(int id in p.ClientIds.Distinct())
                {
                    Client client = context.Clients.Find(id);

                    if(client == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    ProductClient productClient = new ProductClient()
                    {
                        Client = client
                    };

                    product.ProductsClients.Add(productClient);
                    
                }
                products.Add(product);
                sb.AppendLine(String.Format(SuccessfullyImportedProducts, product.Name, product.ProductsClients.Count));
            }
            context.Products.AddRange(products);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

            
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
