using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO;
using Ambev.DeveloperEvaluation.Persistence.Handlers.Sales;
using AutoMapper;
using Bogus;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities;
using NSubstitute.ReceivedExtensions;
using Newtonsoft.Json;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class SaleDeletedItemEventHandlerTests
    {
        private readonly ILogger<SaleDeletedItemEvent> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;
        private readonly IMapper _mapper;
        private readonly SaleDeletedItemEventHandler _handler;
        private readonly Faker _faker;

        public SaleDeletedItemEventHandlerTests()
        {
            _logger = Substitute.For<ILogger<SaleDeletedItemEvent>>();
            _saleRepository = Substitute.For<ISaleRepository>();
            _requestLogService = Substitute.For<IRequestLogService>();
            _mapper = Substitute.For<IMapper>();
            _faker = new Faker();

            _handler = new SaleDeletedItemEventHandler(_logger, _saleRepository, _requestLogService, _mapper);
        }

        [Fact]
        public async Task Handle_Should_LogError_When_SaleItem_NotFound()
        {
            var eventMessage = new SaleDeletedItemEvent(_faker.Random.Guid(), DateTime.Now);
            _saleRepository.GetSaleItemByIdAsync(eventMessage.Id).Returns(Task.FromResult<SaleItem?>(null));

            await _handler.Handle(eventMessage);

            _logger.Received().Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(msg => msg.ToString().Contains("Error SaleItem ID not found")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_Should_LogError_When_Sale_NotFound()
        {
            var saleItem = new SaleItem(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Commerce.ProductName(), _faker.Random.Int(1, 20), _faker.Random.Decimal(10, 200));
            var eventMessage = new SaleDeletedItemEvent(saleItem.Id, DateTime.Now);

            _saleRepository.GetSaleItemByIdAsync(eventMessage.Id).Returns(Task.FromResult(saleItem));
            _saleRepository.GetByIdAsync(saleItem.SaleId).Returns(Task.FromResult<Sale?>(null));

            await _handler.Handle(eventMessage);

            _logger.Received().Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(msg => msg.ToString().Contains("Error Sale ID not found")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_Should_Process_Deletion_Successfully()
        {
            var saleId = _faker.Random.Guid();
            var saleItemId = _faker.Random.Guid();
            var saleItem = new SaleItem(saleItemId, saleId, _faker.Commerce.ProductName(), _faker.Random.Int(1, 20), _faker.Random.Decimal(10, 200));
            var sale = new Sale(saleId, _faker.Random.AlphaNumeric(10), _faker.Person.FullName, _faker.Company.CompanyName(), DateTime.UtcNow);
            sale.UpdateItems(new List<SaleItem> { saleItem });
            var eventMessage = new SaleDeletedItemEvent(saleItemId, DateTime.Now);
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var saleLog = JsonConvert.DeserializeObject<SaleLog>(JsonConvert.SerializeObject(new
            {
                Items = new List<SaleItemLog>()
            }), settings);

            _saleRepository.GetSaleItemByIdAsync(saleItemId).Returns(Task.FromResult(saleItem));
            _saleRepository.GetByIdAsync(saleId).Returns(Task.FromResult(sale));
            _mapper.Map<SaleLog>(sale).Returns(saleLog);

            await _handler.Handle(eventMessage);

            await _saleRepository.Received(1).DeleteSaleItemsAsync(Arg.Is<List<SaleItem>>(items => items.Count == 1 && items[0].Id == saleItemId));
            await _requestLogService.Received(1).UpdateRequestAsync(saleId.ToString(), Arg.Any<BsonDocument>());
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
