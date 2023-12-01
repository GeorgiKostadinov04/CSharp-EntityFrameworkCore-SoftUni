namespace Invoices.DataProcessor
{
    using AutoMapper.QueryableExtensions;
    using AutoMapper;
    using Invoices.Data;
    using Invoices.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Serialization;
    using Invoices.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvoicesProfile>();
            });
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportClientsDto[]), new XmlRootAttribute("Clients"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter sw = new StringWriter(sb);

            ExportClientsDto[] clientsDtos = context
                .Clients
                .Where(c => c.Invoices.Any(ci => ci.IssueDate >= date))
                .ProjectTo<ExportClientsDto>(config)
                .OrderByDescending(c => c.InvoicesCount)
                .ThenBy(c => c.Name)
                .ToArray();
            xmlSerializer.Serialize(sw, clientsDtos, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {

            var products = context.Products
                .Where(p => p.ProductsClients.Any(pc => pc.Client.Name.Length >= nameLength))
                .ToArray()
                .Select(p => new
                {
                    p.Name,
                    p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients.Where(pc => pc.Client.Name.Length >= nameLength)
                    .ToArray()
                    .OrderBy(pc => pc.Client.Name)
                    .Select(pc => new
                    {
                        Name = pc.Client.Name,
                        NumberVat = pc.Client.NumberVat
                    })
                    .ToArray()
                })
                .OrderByDescending(p => p.Clients.Length)
                .ThenBy(p => p.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }
    }
}