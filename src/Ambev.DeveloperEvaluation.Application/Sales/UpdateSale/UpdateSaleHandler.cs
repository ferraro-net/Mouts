using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Persistence;
using Newtonsoft.Json;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, string>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<string> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return string.Join("\n", validationResult.Errors);

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, command.Branch, cancellationToken);

        if (existingSale != null && existingSale.Id != command.Id)
            return $"Sale with SaleNumber {command.SaleNumber} and Branch {command.Branch} already exists";

        var saleEvent = new SaleUpdatedEvent(command.Id, JsonConvert.SerializeObject(command), DateTime.Now);
        await _eventDispatcher.Publish(saleEvent);

        return "";
    }
}
