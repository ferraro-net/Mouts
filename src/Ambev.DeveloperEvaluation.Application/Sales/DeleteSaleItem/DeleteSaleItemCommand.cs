using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSaleItem;

public record DeleteSaleItemCommand : IRequest<DeleteSaleItemResponse>
{
    public Guid Id { get; }

    public DeleteSaleItemCommand(Guid id)
    {
        Id = id;
    }
}
