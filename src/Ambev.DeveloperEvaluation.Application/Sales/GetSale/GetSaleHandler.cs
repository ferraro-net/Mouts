using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO;
using MongoDB.Bson.Serialization;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly IMapper _mapper;
    private readonly IRequestLogService _requestLogService;

    public GetSaleHandler(IMapper mapper, IRequestLogService requestLogService)
    {
        _mapper = mapper;
        _requestLogService = requestLogService;
    }

    public async Task<GetSaleResult?> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var requestSaleLog = await _requestLogService.GetRequestByIdAsync(request.Id.ToString());

        if (requestSaleLog == null)
            return null;

        var saleLog = BsonSerializer.Deserialize<SaleLog>(requestSaleLog.RequestBody);

        if (saleLog == null)
            return null;

        return _mapper.Map<GetSaleResult>(saleLog);
    }
}
