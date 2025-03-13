using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Persistence;
using Newtonsoft.Json;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, command.Branch, cancellationToken);

        if (existingSale != null)
            throw new InvalidOperationException($"Sale with SaleNumber {command.SaleNumber} and Branch {command.Branch} already exists");

        var saleId = Guid.NewGuid();

        var saleEvent = new SaleCreatedEvent(saleId, JsonConvert.SerializeObject(command), DateTime.Now);
        await _eventDispatcher.Publish(saleEvent);

        return new CreateSaleResult() { Id = saleId };
    }
}
