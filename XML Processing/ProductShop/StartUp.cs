using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.DynamicProxy.Generators;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            string inputXml = File.ReadAllText("C:\\Users\\HP\\Documents\\GitHub\\CSharp-EntityFrameworkCore-SoftUni\\XML Processing\\ProductShop\\Datasets\\categories-products.xml");

            Console.WriteLine(GetProductsInRange(context));
        }


        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            return new Mapper(cfg);
        }


        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            ImportUserDTO[] importUserDTOs = new XmlHelper().Deserialize<ImportUserDTO[]>(inputXml, "Users");

            User[] users = mapper.Map<User[]>(importUserDTOs);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }


        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            ImportProductDTO[] importProductDTO = new XmlHelper()
                .Deserialize<ImportProductDTO[]>(inputXml, "Products");

            Product[] products = mapper.Map<Product[]>(importProductDTO);
            var userIds = context.Users.Select(u => u.Id).ToArray();
            context.Products.AddRange(products.Where(p => userIds.Contains(p.SellerId) && userIds.Contains(p.BuyerId)));
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";

        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            ImportCategoryDTO[] importCategoryDTOs = new XmlHelper().Deserialize<ImportCategoryDTO[]>(inputXml, "Categories");

            Category[] categories = mapper.Map<Category[]>(importCategoryDTOs);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            ImportCategoriesProductsDTO[] categoryProductDtos = new XmlHelper()
            .Deserialize<ImportCategoriesProductsDTO[]>(inputXml, "CategoryProducts");

            CategoryProduct[] categoryProducts = mapper
                .Map<CategoryProduct[]>(categoryProductDtos);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var mapper = GetMapper();

            ExportProductsInRangeDTO[] products = context
            .Products
            .AsNoTracking()
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .OrderBy(p => p.Price)
            .Take(10)
            .ProjectTo<ExportProductsInRangeDTO>(mapper.ConfigurationProvider)
            .ToArray();

            return new XmlHelper()
                .Serialize(products, "Products");
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var mapper = GetMapper();

            var soldProducts = context
                .Users
                .AsNoTracking()
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportUserDTO>(mapper.ConfigurationProvider)
                .ToArray();

            return new XmlHelper()
                .Serialize(soldProducts, "Users");
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var mapper = GetMapper();

            var categories = context
                .Categories
                .AsNoTracking()
                .ProjectTo<ExportCategoryDTO>(mapper.ConfigurationProvider)
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            return new XmlHelper()
                .Serialize(categories, "Categories");
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            ExportUserWithProductsDTO[] users = context
                .Users
                .AsNoTracking()
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new ExportUserWithProductsDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    soldProducts = new ExportSoldProductsWithCount()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                            .Select(p => new ExportProductNamePriceDto()
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .OrderByDescending(p => p.Price)
                            .ToArray()
                    }
                })
                .ToArray();

            ExportUsersWithCountDTO resultDto = new()
            {
                Count = users.Length,
                Users = users
                    .Take(10)
                    .ToArray()
            };

            return new XmlHelper()
                .Serialize(resultDto, "Users");
        }
    }
}