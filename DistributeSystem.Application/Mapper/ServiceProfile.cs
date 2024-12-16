using AutoMapper;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Product;
using DistributeSystem.Domain.Entities;

namespace DistributeSystem.Application.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        // V1
        CreateMap<Product, Response.ProductResponse>().ReverseMap();
        CreateMap<PagedResult<Product>, PagedResult<Response.ProductResponse>>().ReverseMap();

        //// V2
        //CreateMap<Product, Contract.Services.V2.Product.Response.ProductResponse>().ReverseMap();
    }
}
