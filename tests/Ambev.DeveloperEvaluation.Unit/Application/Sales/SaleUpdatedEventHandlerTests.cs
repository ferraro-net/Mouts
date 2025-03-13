using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Persistence.Handlers.Sales;
using Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO;
using AutoMapper;
using Bogus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class SaleUpdatedEventHandlerTests
    {
        private readonly ILogger<SaleUpdatedEvent> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IRequestLogService _requestLogService;
        private readonly IMapper _mapper;
        private readonly SaleUpdatedEventHandler _handler;
        private readonly Faker _faker;

        public SaleUpdatedEventHandlerTests()
        {
            _logger = Substitute.For<ILogger<SaleUpdatedEvent>>();
            _saleRepository = Substitute.For<ISaleRepository>();
            _requestLogService = Substitute.For<IRequestLogService>();
            _mapper = Substitute.For<IMapper>();

            _handler = new SaleUpdatedEventHandler(_logger, _saleRepository, _requestLogService, _mapper);
            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_Should_LogWarning_When_SaleIsNull()
        {
            var message = new SaleUpdatedEvent(Guid.NewGuid(), null, DateTime.Now);

            await _handler.Handle(message);

            _logger.Received().LogWarning("SaleUpdatedEvent received with null Sale object.");
        }

        [Fact]
        public async Task Handle_Should_LogError_When_SaleNotFound()
        {
            var message = new SaleUpdatedEvent(Guid.NewGuid(), "{}", DateTime.Now);
            _saleRepository.GetByIdAsync(message.Id).Returns(Task.FromResult<Sale>(null));

            await _handler.Handle(message);

            _logger.Received().LogError("Sale ID not found to update.");
        }

        [Fact]
        public async Task Handle_Should_LogError_When_DeserializationFails()
        {
            var message = new SaleUpdatedEvent(Guid.NewGuid(), "{ invalid json", DateTime.Now);
            _saleRepository.GetByIdAsync(message.Id).Returns(new Sale(message.Id, "12345", "Cliente X", "Filial Y", DateTime.UtcNow));

            await _handler.Handle(message);

            _logger.Received().LogError(Arg.Any<JsonException>(), "Failed to deserialize message from event message: Invalid JSON format.");
        }

        [Fact]
        public async Task Handle_Should_UpdateSaleCustomer()
        {
            var saleId = Guid.NewGuid();
            var oldSale = new Sale(saleId, "12345", "Cliente Antigo", "Filial Y", DateTime.UtcNow);
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var updatedSaleLog = JsonConvert.DeserializeObject<SaleLog>(JsonConvert.SerializeObject(new
            {
                Customer = "Cliente Novo",
                Items = new List<SaleItemLog>()
            }), settings);

            var message = new SaleUpdatedEvent(saleId, JsonConvert.SerializeObject(updatedSaleLog), DateTime.Now);

            _saleRepository.GetByIdAsync(saleId).Returns(oldSale);
            _mapper.Map<SaleLog>(oldSale).Returns(updatedSaleLog);

            await _handler.Handle(message);

            Assert.Equal("Cliente Novo", oldSale.Customer);
            await _saleRepository.Received().UpdateAsync(oldSale);
        }

        [Fact]
        public async Task Handle_Should_Delete_Items_Not_In_Event()
        {
            var saleId = Guid.NewGuid();
            var oldItem = new SaleItem(Guid.NewGuid(), saleId, "Produto Antigo", 1, 10);
            var oldSale = new Sale(saleId, "12345", "Cliente", "Filial", DateTime.UtcNow);
            oldSale.UpdateItems(new List<SaleItem> { oldItem });

            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var updatedSaleLog = JsonConvert.DeserializeObject<SaleLog>(JsonConvert.SerializeObject(new
            {
                Customer = "Cliente",
                Items = new List<SaleItemLog>()
            }), settings);

            var message = new SaleUpdatedEvent(saleId, JsonConvert.SerializeObject(updatedSaleLog), DateTime.Now);

            _saleRepository.GetByIdAsync(saleId).Returns(oldSale);
            _mapper.Map<SaleLog>(oldSale).Returns(updatedSaleLog);

            await _handler.Handle(message);

            await _saleRepository.Received().DeleteSaleItemsAsync(Arg.Is<List<SaleItem>>(list => list.Contains(oldItem)));
        }

        [Fact]
        public async Task Handle_Should_Create_New_Items()
        {
            var saleId = Guid.NewGuid();
            var oldSale = new Sale(saleId, "12345", "Cliente", "Filial", DateTime.UtcNow);
            var newItemLog = new SaleItemLog { Product = "Produto Novo", Quantity = 2, UnitPrice = 5 };

            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var updatedSaleLog = JsonConvert.DeserializeObject<SaleLog>(JsonConvert.SerializeObject(new
            {
                Customer = "Cliente",
                Items = new List<SaleItemLog>() { newItemLog }
            }), settings);

            var message = new SaleUpdatedEvent(saleId, JsonConvert.SerializeObject(updatedSaleLog), DateTime.Now);

            _saleRepository.GetByIdAsync(saleId).Returns(oldSale);
            _mapper.Map<SaleLog>(oldSale).Returns(updatedSaleLog);

            await _handler.Handle(message);

            await _saleRepository.Received().CreateSaleItemAsync(Arg.Is<SaleItem>(item =>
                item.Product == "Produto Novo" && item.Quantity == 2 && item.UnitPrice == 5));
        }
    }
}
