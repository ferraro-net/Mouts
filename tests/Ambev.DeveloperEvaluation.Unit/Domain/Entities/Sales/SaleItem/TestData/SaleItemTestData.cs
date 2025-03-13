using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.SaleItem.TestData
{
    public class SaleItemTestData
    {
        public Guid Id { get; }
        public Guid SaleId { get; }
        public string Product { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        public decimal ExpectedDiscount { get; }
        public decimal ExpectedTotalAmount { get; }

        public SaleItemTestData()
        {
            var faker = new Faker();

            Id = Guid.NewGuid();
            SaleId = Guid.NewGuid();
            Product = faker.Commerce.ProductName();
            Quantity = faker.Random.Int(1, 20);
            UnitPrice = faker.Random.Decimal(1, 100);
            ExpectedDiscount = CalculateDiscount(Quantity);
            ExpectedTotalAmount = (UnitPrice * Quantity) * (1 - ExpectedDiscount);
        }
        private decimal CalculateDiscount(int quantity)
        {
            if (quantity <= 4) return 0;
            if (quantity >= 10) return 0.20m;
            return 0.10m;
        }
    }
}
