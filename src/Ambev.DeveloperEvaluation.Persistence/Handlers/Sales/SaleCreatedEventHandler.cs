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
    public class SaleCreatedEventHandler : IHandleMessages<SaleCreatedEvent>
    {
        private readonly ILogger<SaleCreatedEventHandler> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;
        private readonly IMapper _mapper;


        public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger, ISaleRepository saleRepository, IRequestLogService requestLogService, IMapper mapper)
        {
            _logger = logger;
            _saleRepository = saleRepository;
            _requestLogService = requestLogService;
            _mapper = mapper;
        }

        public async Task Handle(SaleCreatedEvent message)
        {
            try
            {
                _logger.LogInformation($" [x] SaleCreatedEvent received: {message.Id}");

                if (message?.Sale == null)
                {
                    _logger.LogWarning("SaleCreatedEvent received with null Sale object.");
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

                var sale = new Sale(message.Id, saleLog.SaleNumber, saleLog.Customer, saleLog.Branch, saleLog.SaleDate);
                var listItems = new List<SaleItem>();

                saleLog.Id = sale.Id.ToString();

                foreach (var item in saleLog.Items)
                {
                    var saleItem = new SaleItem(Guid.NewGuid(), sale.Id, item.Product, item.Quantity, item.UnitPrice);
                    listItems.Add(saleItem);
                    item.Id = saleItem.Id.ToString();
                    item.Discount = saleItem.Discount;
                    item.TotalAmount = saleItem.TotalAmount;
                }

                sale.UpdateItems(listItems);

                var createdSale = await _saleRepository.CreateAsync(sale);
                
                var saleDocument = _mapper.Map<SaleLog>(sale);

                await _requestLogService.SaveRequestAsync(sale.Id.ToString(), saleDocument.ToBsonDocument());

                _logger.LogInformation($" [✓] Processed Sale Creation: {message.Id}");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error processing SaleCreatedEvent: {Id}", message.Id);
                await _saleRepository.DeleteAsync(message.Id);
                throw;
            }
        }
    }
}
