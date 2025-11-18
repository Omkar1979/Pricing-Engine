using AutoMapper;
using SmartInventory.Api.DTOs;
using SmartInventory.Api.Models;

namespace SmartInventory.Api.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}

