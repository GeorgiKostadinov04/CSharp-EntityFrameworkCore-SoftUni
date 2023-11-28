namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportDispatcherDto[]), new XmlRootAttribute("Despatchers"));

            using StringReader stringReader = new StringReader(xmlString);

            ImportDispatcherDto[] despatcherDtos = (ImportDispatcherDto[])xmlSerializer.Deserialize(stringReader);

            List<Despatcher> despatchers = new List<Despatcher>();

            foreach (ImportDispatcherDto despatcherDto in despatcherDtos)
            {
                if (!IsValid(despatcherDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                string position = despatcherDto.Position;
                bool isPositionInvalid = string.IsNullOrEmpty(position);

                if (isPositionInvalid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Despatcher d = new Despatcher()
                {
                    Name = despatcherDto.Name,
                    Position = position
                };

                foreach (ImportTruckDto truckDto in despatcherDto.Trucks)
                {
                    if (!IsValid(truckDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck t = new Truck()
                    {
                        RegistrationNumber = truckDto.RegistrationNumber,
                        VinNumber = truckDto.VinNumber,
                        TankCapacity = truckDto.TankCapacity,
                        CargoCapacity = truckDto.CargoCapacity,
                        CategoryType = (CategoryType)truckDto.CategoryType,
                        MakeType = (MakeType)truckDto.MakeType
                    };

                    d.Trucks.Add(t);
                }
                despatchers.Add(d);
                sb.AppendLine(String.Format(SuccessfullyImportedDespatcher, d.Name, d.Trucks.Count));
            }
            context.Despatchers.AddRange(despatchers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportClientDto[] importClientDtos = JsonConvert.DeserializeObject<ImportClientDto[]>(jsonString);

            List<Client> clientList = new List<Client>();   
            foreach(ImportClientDto c in importClientDtos)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Client clientToAdd = new Client()
                {
                    Name = c.Name,
                    Nationality = c.Nationality,
                    Type = c.Type

                };

                if(c.Type == "usual")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                foreach(int id in c.Trucks.Distinct())
                {
                    Truck t = context.Trucks.Find(id);
                    if(t == null) 
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    clientToAdd.ClientsTrucks.Add(new ClientTruck
                    {
                        Truck = t
                    });

                }
                clientList.Add(clientToAdd);
                sb.AppendLine(String.Format(SuccessfullyImportedClient, clientToAdd.Name, clientToAdd.ClientsTrucks.Count));
               
            }
            context.Clients.AddRange(clientList);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}