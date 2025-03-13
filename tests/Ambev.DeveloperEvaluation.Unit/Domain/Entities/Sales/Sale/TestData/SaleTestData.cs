using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.SaleItem.TestData;
using Bogus;
using Bogus.DataSets;
using Xunit.Abstractions;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.Sale.TestData
{
    public class SaleTestData
    {      
        public Guid Id { get; }
        public string SaleNumber { get; }
        public DateTime SaleDate { get; }
        public string Customer { get; }
        public string Branch { get; }
        public List<SaleItemTestData> Items { get; }
        public decimal ExpectedTotalSaleAmount { get; }

        public SaleTestData()
        {
            var faker = new Faker();

            Id = Guid.NewGuid();
            SaleNumber = faker.Random.AlphaNumeric(10);
            SaleDate = faker.Date.Past();
            Customer = faker.Person.FullName;
            Branch = faker.Company.CompanyName();

            Items = new List<SaleItemTestData>();
            for (int i = 0; i < faker.Random.Int(1, 5); i++)
            {
                Items.Add(new SaleItemTestData());
            }

            ExpectedTotalSaleAmount = Items.Sum(item => item.ExpectedTotalAmount);
        }
    }
}
