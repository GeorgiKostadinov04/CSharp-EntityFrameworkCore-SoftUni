using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            string supplierJson = File.ReadAllText("C:\\Users\\HP\\Documents\\GitHub\\CSharp-EntityFrameworkCore-SoftUni\\JSON Processing\\CarDealer\\Datasets\\sales.json");

            Console.WriteLine(GetCarsWithTheirListOfParts(context));
        }


        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());

            IMapper mapper = new Mapper(config);



            SupplierDTO[] suppliersDTOs = JsonConvert.DeserializeObject<SupplierDTO[]>(inputJson);

            Supplier[] suppliers = mapper.Map<Supplier[]>(suppliersDTOs);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";


        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg=>cfg.AddProfile<CarDealerProfile>());

            IMapper mapper = new Mapper(config);

            PartDTO[] partsDTOs = JsonConvert.DeserializeObject<PartDTO[]>(inputJson);

            ICollection<Part> parts = new List<Part>();

            foreach (var part in partsDTOs)
            {
                if (context.Suppliers.Any(s => s.Id == part.SupplierId))
                {
                    parts.Add(mapper.Map<Part>(part));
                }
            }

            
            context.Parts.AddRangeAsync(parts);
            context.SaveChanges();

            
            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());

            IMapper mapper = new Mapper(config);

            CarDTO[] carsDTOs = JsonConvert.DeserializeObject<CarDTO[]>(inputJson);

            ICollection<Car> carsToAdd = new HashSet<Car>();

            foreach (var carDto in carsDTOs)
            {
                Car currentCar = mapper.Map<Car>(carDto);

                foreach (var id in carDto.PartsIds)
                {
                    if (context.Parts.Any(p => p.Id == id))
                    {
                        currentCar.PartsCars.Add(new PartCar
                        {
                            PartId = id,
                        });
                    }
                }

                carsToAdd.Add(currentCar);
            }
            context.Cars.AddRange(carsToAdd);

            context.SaveChanges();

            return $"Successfully imported {carsToAdd.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            CarDTO[] customerDtos = JsonConvert.DeserializeObject<CarDTO[]>(inputJson);

            //Mapping the Customers from their DTOs
            Customer[] customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            //Adding the Customers
            context.Customers.AddRange(customers);
            context.SaveChanges();

            //Output
            return $"Successfully imported {customers.Length}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            SaleDTO[] salesDtos = JsonConvert.DeserializeObject<SaleDTO[]>(inputJson);
            Sale[] sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            //Adding the Sales
            context.Sales.AddRange(sales);
            context.SaveChanges();

            //Output
            return $"Successfully imported {sales.Length}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver.ToString())
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo),
                    c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert
                .SerializeObject(customers, Formatting.Indented);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            return JsonConvert.SerializeObject (suppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                   car = new
                   {
                       c.Make,
                       c.Model,
                       c.TraveledDistance
                   },

                   parts = c.PartsCars
                    .Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("f2")
                    })
                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .AsNoTracking()
                .Where(c=>c.Sales.Any(s=>s.Car != null))
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(s=>s.Car != null),
                    SpentMoney = c.Sales
                        .SelectMany(s=>s.Car.PartsCars)
                        .Sum(pc=>pc.Part.Price)

                })
                .OrderByDescending(c=>c.SpentMoney)
                .ThenByDescending(c=>c.BoughtCars)
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver = ConfigureCamelCase()
            });

        }

        private static IContractResolver ConfigureCamelCase()
        => new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy(false, true)
        };

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var cars = context
            .Sales
            .AsNoTracking()
            .Select(c => new
            {
                car = new
                {
                    c.Car.Make,
                    c.Car.Model,
                    c.Car.TraveledDistance
                },

                customerName = c.Customer.Name,
                discount = c.Discount.ToString("f2"),
                price = c.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                priceWithDiscount = (c.Car.PartsCars.Sum(pc => pc.Part.Price)
                                    - ((c.Car.PartsCars.Sum(pc => pc.Part.Price) * c.Discount) / 100)).ToString("f2")
            })
            .Take(10)
            .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }
    }



}