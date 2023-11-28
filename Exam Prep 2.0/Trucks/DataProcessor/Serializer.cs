namespace Trucks.DataProcessor
{
    using AutoMapper.QueryableExtensions;
    using AutoMapper;
    using Data;
    using System.Text;
    using System.Text.Json;
    using System.Xml.Serialization;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TrucksProfile>();
            });

            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportDespatcherDto[]), new XmlRootAttribute("Despatchers"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter sw = new StringWriter(sb);

            ExportDespatcherDto[] despatcherDtos =
                context
                .Despatchers
                .Where(d => d.Trucks.Any())
                .ProjectTo<ExportDespatcherDto>(config)
                .OrderByDescending(d => d.TrucksCount)
                .ThenBy(d => d.Name)
                .ToArray();
            xmlSerializer.Serialize(sw, despatcherDtos, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Where(c => c.ClientsTrucks.Any(ct => ct.Truck.TankCapacity >= capacity))
                .ToArray()
                .Select(c => new
                {
                    c.Name,
                    Trucks = c.ClientsTrucks.Where(ct=>ct.Truck.TankCapacity >= capacity)
                    .ToArray()
                    .Select(ct => new
                    {
                        TruckRegistrationNumber = ct.Truck.RegistrationNumber,
                        VinNumber = ct.Truck.VinNumber,
                        TankCapacity = ct.Truck.TankCapacity,
                        CargoCapacity = ct.Truck.CargoCapacity,
                        CategoryType = ct.Truck.CategoryType.ToString(),
                        MakeType = ct.Truck.MakeType.ToString(),
                    })
                    .OrderBy(t => t.MakeType)
                    .ThenByDescending(t => t.CargoCapacity)
                    .ToArray()
                })
                .OrderByDescending(c => c.Trucks.Length)
                .ThenBy(c => c.Name)
                .Take(10)
                .ToArray();

            return JsonSerializer.Serialize(clients);
        }
    }
}
