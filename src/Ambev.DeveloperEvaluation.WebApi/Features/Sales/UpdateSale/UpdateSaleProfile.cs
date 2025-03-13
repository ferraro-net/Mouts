using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();

        CreateMap<Sale, SaleLog>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
              .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItem, SaleItemLog>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
    }
}
