using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Persistence.Handlers.Sales
{
    public class SaleDeletedEventHandler : IHandleMessages<SaleDeletedEvent>
    {
        private readonly ILogger<SaleDeletedEvent> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;

        public SaleDeletedEventHandler(ILogger<SaleDeletedEvent> logger, ISaleRepository saleRepository, IRequestLogService requestLogService)
        {
            _logger = logger;
            _saleRepository = saleRepository;
            _requestLogService = requestLogService;
        }

        public async Task Handle(SaleDeletedEvent message)
        {
            try
            {
                _logger.LogInformation($" [x] SaleDeletedEvent received: {message.Id}");

                await _requestLogService.DeleteRequestAsync(message.Id.ToString());
                await _saleRepository.DeleteAsync(message.Id);

                _logger.LogInformation($" [✓] Processed Sale Deleted: {message.Id}");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error processing SaleDeletedEvent: {SaleId}", message.Id);
                throw;
            }
        }
    }
}
