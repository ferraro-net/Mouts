using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSaleItem;

public class DeleteSaleItemProfile : Profile
{
    public DeleteSaleItemProfile()
    {
        CreateMap<Guid, Application.Sales.DeleteSaleItem.DeleteSaleItemCommand>()
            .ConstructUsing(id => new Application.Sales.DeleteSaleItem.DeleteSaleItemCommand(id));
    }
}
