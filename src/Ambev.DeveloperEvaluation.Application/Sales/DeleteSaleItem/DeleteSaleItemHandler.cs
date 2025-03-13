using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Persistence;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSaleItem;

public class DeleteSaleItemHandler : IRequestHandler<DeleteSaleItemCommand, DeleteSaleItemResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;

    public DeleteSaleItemHandler(ISaleRepository saleRepository, IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<DeleteSaleItemResponse> Handle(DeleteSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var saleEvent = new SaleDeletedItemEvent(request.Id, DateTime.Now);
        await _eventDispatcher.Publish(saleEvent);

        return new DeleteSaleItemResponse { Success = true };
    }
}
