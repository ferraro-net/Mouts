using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using Rebus.Handlers;


namespace Ambev.DeveloperEvaluation.Persistence.Handlers.Sales
{
    public class SaleUpdatedEventHandler : IHandleMessages<SaleUpdatedEvent>
    {
        private readonly ILogger<SaleUpdatedEvent> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;
        private readonly IMapper _mapper;

        public SaleUpdatedEventHandler(ILogger<SaleUpdatedEvent> logger, ISaleRepository saleRepository, IRequestLogService requestLogService, IMapper mapper)
        {
            _logger = logger;
            _saleRepository = saleRepository;
            _requestLogService = requestLogService;
            _mapper = mapper;
        }

        public async Task Handle(SaleUpdatedEvent message)
        {
            try
            {
                _logger.LogInformation($" [x] SaleUpdatedEvent received: {message.Id}");

                if (message?.Sale == null)
                {
                    _logger.LogWarning("SaleUpdatedEvent received with null Sale object.");
                    return;
                }

                var sale = await _saleRepository.GetByIdAsync(message.Id);

                if (sale == null)
                {
                    _logger.LogError("Sale ID not found to update.");
                    return;
                }

                SaleLog saleLog;
                try
                {
                    saleLog = JsonConvert.DeserializeObject<SaleLog>(message.Sale.ToString());

                    if (saleLog == null)
                    {
                        _logger.LogError("Failed to deserialize message from event message: Deserialization returned null.");
                        return;
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize message from event message: Invalid JSON format.");
                    return;
                }

                sale.UpdateCustomer(saleLog.Customer);

                await _saleRepository.UpdateAsync(sale);

                var itemsToInsert = new List<SaleItem>();

                var itemsToDelete = sale.Items.Where(existingItem => !saleLog.Items.Any(logItem => logItem.Product.Trim().ToUpper() == existingItem.Product.Trim().ToUpper())).ToList();

                foreach (var item in saleLog.Items)
                {
                    var existingItem = sale.Items.FirstOrDefault(i => i.Product.Trim().ToUpper() == item.Product.Trim().ToUpper());

                    if (existingItem != null)
                    {
                        if (existingItem.Quantity != item.Quantity || existingItem.UnitPrice != item.UnitPrice)
                        {
                            existingItem.UpdateItem(item.Quantity, item.UnitPrice);
                            await _saleRepository.UpdateSaleItemAsync(existingItem);
                        }
                    }
                    else
                    {
                        await _saleRepository.CreateSaleItemAsync(new SaleItem(string.IsNullOrEmpty(item.Id) ? Guid.NewGuid() : Guid.Parse(item.Id), message.Id, item.Product, item.Quantity, item.UnitPrice));
                    }
                }

                await _saleRepository.DeleteSaleItemsAsync(itemsToDelete);

                var saleDocument = _mapper.Map<SaleLog>(sale);

                await _requestLogService.UpdateRequestAsync(sale.Id.ToString(), saleDocument.ToBsonDocument());

                _logger.LogInformation($" [✓] Processed Sale Updated: {message.Id}");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error processing SaleUpdatedEvent: {Id}", message.Id);
                throw;
            }
        }
    }
}
