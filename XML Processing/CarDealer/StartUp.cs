using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            string xml = File.ReadAllText("C:\\Users\\HP\\Documents\\GitHub\\CSharp-EntityFrameworkCore-SoftUni\\XML Processing\\CarDealer\\Datasets\\sales.xml");

            Console.WriteLine(GetLocalSuppliers(context));
        }

        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(c=>c.AddProfile<CarDealerProfile>());
            return new Mapper(cfg);
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDTO[]),
                new XmlRootAttribute("Suppliers"));

            using var reader = new StringReader(inputXml);

            ImportSupplierDTO[] importSupplierDTOs = (ImportSupplierDTO[])xmlSerializer.Deserialize(reader);

            var mapper = GetMapper();

            Supplier[] suppliers = mapper.Map<Supplier[]>(importSupplierDTOs);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDTO[]),
                new XmlRootAttribute("Parts"));

            using var reader = new StringReader(inputXml);

            ImportPartDTO[] importPartDTOs = (ImportPartDTO[])xmlSerializer.Deserialize(reader);

            var supplierIds = context.Suppliers
                .Select(x => x.Id)
                .ToArray();

            var mapper = GetMapper();

            var parts = mapper.Map<Part[]>(importPartDTOs.Where(p=>supplierIds.Contains(p.SupplierId)));

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDTO[]), 
                new XmlRootAttribute("Cars"));

            using var reader = new StringReader(inputXml);

            ImportCarDTO[] importCarDTOs = (ImportCarDTO[])xmlSerializer.Deserialize(reader);

            var mapper = GetMapper();

            List<Car> cars = new List<Car>();

            foreach(var carDTO in importCarDTOs)
            {
                Car car = mapper.Map<Car> (carDTO);

                int[] carPartsIds = carDTO.PartsIds.Select(x=> x.Id).Distinct().ToArray();

                var carParts = new List<PartCar>();

                foreach(var id in carPartsIds)
                {
                    carParts.Add(new PartCar
                    {
                        Car = car,
                        PartId = id
                    });
                }

                car.PartsCars = carParts;
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}";

        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDTO[]),
                new XmlRootAttribute("Customers"));

            using var reader = new StringReader(inputXml);

            ImportCustomerDTO[] importCustomerDTOs = (ImportCustomerDTO[])xmlSerializer.Deserialize(reader);

            var mapper = GetMapper();

            Customer[] customers = mapper.Map<Customer[]>(importCustomerDTOs);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSaleDTO[]),
                new XmlRootAttribute("Sales"));

            using var reader = new StringReader(inputXml);

            ImportSaleDTO[] importSaleDTOs = (ImportSaleDTO[])xmlSerializer.Deserialize(reader);

            var mapper = GetMapper();

            var carIds = context.Cars.Select(c=>c.Id).ToArray();

            Sale[] sales = mapper.Map<Sale[]>(importSaleDTOs.Where(s=>carIds.Contains(s.CarId)));

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var mapper = GetMapper();

            var cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model).Take(10)
                .ProjectTo<ExportCarsWithDistance>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistance[]),
                new XmlRootAttribute("cars"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty,string.Empty);

            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, cars, xsn);
            }

            return stringBuilder.ToString().TrimEnd();
        }


        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var mapper = GetMapper();

            var BMWcars = context.Cars
            .AsNoTracking()
            .Where(c => c.Make.ToUpper() == "BMW")
            .OrderBy(c => c.Model)
            .ThenByDescending(c => c.TraveledDistance)
            .ProjectTo<ExportBMWCarDto>(mapper.ConfigurationProvider)
            .ToArray();



            return new XmlHelper().Serialize(BMWcars, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var mapper = GetMapper();

            var suppliers = context.Suppliers
                .Where(s=>s.IsImporter == false)
                .AsNoTracking()
                .ProjectTo<ExportSupplierNotImporters>(mapper.ConfigurationProvider)
                .ToArray();

            return new XmlHelper().Serialize(suppliers, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var mapper = GetMapper();

            var carsWithParts = context.Cars
            .AsNoTracking()
            .OrderByDescending(c => c.TraveledDistance)
            .ThenBy(c => c.Model)
            .Take(5)
            .ProjectTo<ExportCarWithPartsDto>(mapper.ConfigurationProvider)
            .ToArray();

            return new XmlHelper()
                .Serialize(carsWithParts, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customersWithSales = context.Customers
                .Include(c => c.Sales)
                .Where(c => c.Sales.Any())
                .ToArray();

            var customers = customersWithSales
                .Select(c => new ExportCustomerDto()
                {
                    Name = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales
                        .SelectMany(s => s.Car.PartsCars)
                        .Join(
                            context.Parts,
                            pc => pc.PartId,
                            p => p.Id,
                            (pc, p) => c.IsYoungDriver
                                ? ((decimal)Math.Round((double)pc.Part.Price * 0.95, 2))
                                : pc.Part.Price
                        )
                        .Sum()
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            return new XmlHelper()
                .Serialize(customers, "customers");
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .AsNoTracking()
                .Select(s => new ExportSaleDto()
                {
                    Car = new ExportCarAttributeDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price)
                })
                .ToArray();

            return new XmlHelper()
                .Serialize(sales, "sales");

        }
    }
}