using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO;
using Ambev.DeveloperEvaluation.Persistence.Handlers.Sales;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using NSubstitute;
using Bogus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class SaleCreatedEventHandlerTests
    {
        private readonly ILogger<SaleCreatedEventHandler> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;
        private readonly IMapper _mapper;
        private readonly SaleCreatedEventHandler _handler;

        public SaleCreatedEventHandlerTests()
        {
            _logger = Substitute.For<ILogger<SaleCreatedEventHandler>>();
            _saleRepository = Substitute.For<ISaleRepository>();
            _requestLogService = Substitute.For<IRequestLogService>();
            _mapper = Substitute.For<IMapper>();

            _handler = new SaleCreatedEventHandler(_logger, _saleRepository, _requestLogService, _mapper);
        }

        [Fact]
        public async Task Handle_Should_LogWarning_When_SaleIsNull()
        {
            var saleEvent = new SaleCreatedEvent(Guid.NewGuid(), null, DateTime.Now);

            await _handler.Handle(saleEvent);

            _logger.Received().Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("SaleCreatedEvent received with null Sale object.")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_Should_LogError_When_DeserializationFails()
        {
            var saleEvent = new SaleCreatedEvent(Guid.NewGuid(), "invalid_json", DateTime.Now);

            await _handler.Handle(saleEvent);

            _logger.Received().Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("Failed to deserialize message from event message: Invalid JSON format.")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_Should_CreateSale_When_DataIsValid()
        {
            var saleId = Guid.NewGuid();
            var saleLog = GenerateFakeSaleLog();
            var saleEvent = new SaleCreatedEvent(saleId, JsonConvert.SerializeObject(saleLog), DateTime.Now);

            _mapper.Map<SaleLog>(Arg.Any<Sale>()).Returns(saleLog);

            await _handler.Handle(saleEvent);

            await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>());
            await _requestLogService.Received(1).SaveRequestAsync(saleId.ToString(), Arg.Any<BsonDocument>());

            _logger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains($"Processed Sale Creation: {saleId}")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_Should_DeleteSale_When_ExceptionOccurs()
        {
            var saleId = Guid.NewGuid();
            var saleLog = GenerateFakeSaleLog();
            var saleEvent = new SaleCreatedEvent(saleId, JsonConvert.SerializeObject(saleLog), DateTime.Now);

            _saleRepository.CreateAsync(Arg.Any<Sale>()).Returns<Task>(x => throw new Exception("Database error"));

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(saleEvent));

            await _saleRepository.Received(1).DeleteAsync(saleId);

            _logger.Received().Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains($"Error processing SaleCreatedEvent: {saleId}")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        private SaleLog GenerateFakeSaleLog()
        {
            return new Faker<SaleLog>()
                .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(10))
                .RuleFor(s => s.Customer, f => f.Person.FullName)
                .RuleFor(s => s.Branch, f => f.Company.CompanyName())
                .RuleFor(s => s.SaleDate, f => f.Date.Past())
                .RuleFor(s => s.Items, f => new List<SaleItemLog>
                {
                new SaleItemLog
                {
                    Product = f.Commerce.ProductName(),
                    Quantity = f.Random.Int(1, 20),
                    UnitPrice = f.Random.Decimal(10, 200)
                }
                })
                .Generate();
        }
    }
}
