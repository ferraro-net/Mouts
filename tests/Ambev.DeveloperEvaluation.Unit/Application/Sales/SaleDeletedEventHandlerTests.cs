using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Persistence.Handlers.Sales;
using Bogus;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class SaleDeletedEventHandlerTests
    {
        private readonly ILogger<SaleDeletedEvent> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;
        private readonly SaleDeletedEventHandler _handler;
        private readonly Faker _faker;

        public SaleDeletedEventHandlerTests()
        {
            _logger = Substitute.For<ILogger<SaleDeletedEvent>>();
            _saleRepository = Substitute.For<ISaleRepository>();
            _requestLogService = Substitute.For<IRequestLogService>();
            _faker = new Faker();

            _handler = new SaleDeletedEventHandler(_logger, _saleRepository, _requestLogService);
        }

        [Fact]
        public async Task Handle_Should_Process_Deletion_Successfully()
        {
            var saleId = _faker.Random.Guid();
            var eventMessage = new SaleDeletedEvent(saleId, DateTime.Now);

            await _handler.Handle(eventMessage);

            await _requestLogService.Received(1).DeleteRequestAsync(saleId.ToString());
            await _saleRepository.Received(1).DeleteAsync(saleId);

            _logger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(msg => msg.ToString().Contains("Processed Sale Deleted")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }
    }
}
