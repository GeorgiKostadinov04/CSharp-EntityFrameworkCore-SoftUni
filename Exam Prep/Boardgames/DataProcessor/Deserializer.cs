namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            
            StringBuilder stringBuilder = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCreatorsDto[]),
                new XmlRootAttribute("Creators"));

            using StringReader stringReader = new StringReader(xmlString);

            ImportCreatorsDto[] importCreatorsDtos = (ImportCreatorsDto[])xmlSerializer.Deserialize(stringReader);

            List<Creator> creatorList = new List<Creator>();
            foreach(var creatorDto in importCreatorsDtos)
            {
                if (!IsValid(creatorDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }
                Creator creator = new Creator()
                {
                    FirstName = creatorDto.FirstName,
                    LastName = creatorDto.LastName,
                };

                foreach(var boardgame in creatorDto.Boardgames)
                {
                    if (!IsValid(boardgame))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame boardgameToAdd = new Boardgame()
                    {
                        Name = boardgame.Name,
                        Rating = boardgame.Rating,
                        YearPublished = boardgame.YearPublished,
                        CategoryType = (CategoryType)boardgame.CategoryType,
                        Mechanics = boardgame.Mechanics,
                    };
                    creator.Boardgames.Add(boardgameToAdd);
                }
                creatorList.Add(creator);
                stringBuilder.AppendLine(String.Format(SuccessfullyImportedCreator, creator.FirstName, creator.LastName, creator.Boardgames.Count));
            }

            context.Creators.AddRange(creatorList);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
            


        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var sellers = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            List<Seller> validSellers = new List<Seller>();

            foreach (ImportSellerDto seller in sellers)
            {
                if (!IsValid(seller))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Seller sellerToAdd = new Seller()
                {
                    Name = seller.Name,
                    Address = seller.Address,
                    Country = seller.Country,
                    Website = seller.Website
                };

                foreach(int id in seller.Boardgames.Distinct())
                {
                    Boardgame boardgameToAdd = context.Boardgames.Find(id);

                    if(boardgameToAdd == null)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    sellerToAdd.BoardgamesSellers.Add(new BoardgameSeller
                    {
                        Boardgame = boardgameToAdd
                    });
                }
                validSellers.Add(sellerToAdd);
                stringBuilder.AppendLine(String.Format(SuccessfullyImportedSeller, sellerToAdd.Name, sellerToAdd.BoardgamesSellers.Count));
            }
            context.Sellers.AddRange(validSellers);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
