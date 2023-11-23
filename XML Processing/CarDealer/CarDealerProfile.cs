using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportSupplierDTO, Supplier>();

            CreateMap<ImportPartDTO, Part>();

            CreateMap<ImportCarDTO, Car>();

            CreateMap<ImportCustomerDTO, Customer>();

            CreateMap<ImportSaleDTO, Sale>();


            CreateMap<Car, ExportCarsWithDistance>();

            CreateMap<Car, ExportBMWCarDto>();

            CreateMap<Supplier, ExportSupplierNotImporters>();

            CreateMap<Car, ExportCarWithPartsDto>()
            .ForMember(d => d.Parts, opt =>
                opt.MapFrom(s => s.PartsCars
                    .Select(pc => pc.Part)
                    .OrderByDescending(p => p.Price)
                    .ToArray()));

            CreateMap<Part, ExportPartDto>();

            CreateMap<Customer, ExportCustomerDto>()
            .ForMember(d => d.BoughtCars, opt =>
                opt.MapFrom(s => s.Sales.Any()));
        }
    }
}
