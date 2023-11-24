using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            // User
            CreateMap<ImportUserDTO, User>();
            CreateMap<User, ExportUserDTO>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

            // Product
            CreateMap<ImportProductDTO, Product>();
            CreateMap<Product, ExportProductsInRangeDTO>()
                .ForMember(d => d.BuyerName,
                            opt => opt.MapFrom(s => $"{s.Buyer.FirstName} {s.Buyer.LastName}"));
            CreateMap<Product, ExportProductNamePriceDto>();

            // Categories
            CreateMap<ImportCategoryDTO, Category>();
            CreateMap<Category, ExportCategoryDTO>()
                .ForMember(d => d.AveragePrice, opt =>
                            opt.MapFrom(s => s.CategoryProducts
                                        .Average(cp => cp.Product.Price)))
                .ForMember(d => d.TotalRevenue, opt =>
                            opt.MapFrom(s => s.CategoryProducts
                                        .Sum(cp => cp.Product.Price)))
                .ForMember(d => d.Count, opt =>
                            opt.MapFrom(s => s.CategoryProducts.Count));

            // CategoryProduct
            CreateMap<ImportCategoriesProductsDTO, CategoryProduct>();

        }

    }
}
