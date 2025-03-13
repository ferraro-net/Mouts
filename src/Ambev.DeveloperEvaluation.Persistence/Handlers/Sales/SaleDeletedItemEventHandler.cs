using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Persistence.Handlers.Sales
{
    public class SaleDeletedItemEventHandler : IHandleMessages<SaleDeletedItemEvent>
    {
        private readonly ILogger<SaleDeletedItemEvent> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;
        private readonly IMapper _mapper;

        public SaleDeletedItemEventHandler(ILogger<SaleDeletedItemEvent> logger, ISaleRepository saleRepository, IRequestLogService requestLogService, IMapper mapper)
        {
            _logger = logger;
            _saleRepository = saleRepository;
            _requestLogService = requestLogService;
            _mapper = mapper;
        }

        public async Task Handle(SaleDeletedItemEvent message)
        {
            try
            {
                _logger.LogInformation($" [x] SaleDeletedEvent received: {message.Id}");

                var item = await _saleRepository.GetSaleItemByIdAsync(message.Id);

                if(item == null) 
                {
                    _logger.LogError("Error SaleItem ID not found: {Id}", message.Id);
                    return;
                }

                var sale = await _saleRepository.GetByIdAsync(item.SaleId);

                if (sale == null)
                {
                    _logger.LogError("Error Sale ID not found: {Id}", message.Id);
                    return;
                }

                await _saleRepository.DeleteSaleItemsAsync(sale.Items.Where(f => f.Id == message.Id).ToList());

                var saleDocument = _mapper.Map<SaleLog>(sale);

                await _requestLogService.UpdateRequestAsync(sale.Id.ToString(), saleDocument.ToBsonDocument());

                _logger.LogInformation($" [✓] Processed Sale Deleted: {message.Id}");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error processing SaleDeletedEvent: {Id}", message.Id);
                throw;
            }
        }
    }
}
