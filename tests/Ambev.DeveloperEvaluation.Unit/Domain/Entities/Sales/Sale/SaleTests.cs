using Xunit;
using FluentAssertions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.Sale.TestData;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.Sale
{
    public class SaleTests
    {
        [Fact]
        public void Sale_Should_Initialize_Correctly()
        {
            var testData = new SaleTestData();

            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);

            sale.Id.Should().Be(testData.Id);
            sale.SaleNumber.Should().Be(testData.SaleNumber);
            sale.Customer.Should().Be(testData.Customer);
            sale.Branch.Should().Be(testData.Branch);
            sale.SaleDate.Should().Be(testData.SaleDate);
            sale.Status.Should().Be(SaleStatus.Active);
            sale.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            sale.Items.Should().BeEmpty();
        }

        [Fact]
        public void Sale_Should_Calculate_TotalSaleAmount_Correctly()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Select(i => new DeveloperEvaluation.Domain.Entities.SaleItem(i.Id, testData.Id, i.Product, i.Quantity, i.UnitPrice)).ToList());

            var totalSaleAmount = sale.TotalSaleAmount;

            totalSaleAmount.Should().BeApproximately(testData.ExpectedTotalSaleAmount, 0.01m);
        }

        [Fact]
        public void UpdateCustomer_Should_Update_Customer_And_Set_UpdatedAt()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            var newCustomer = "Novo Cliente";

            sale.UpdateCustomer(newCustomer);

            sale.Customer.Should().Be(newCustomer);
            sale.UpdatedAt.Should().NotBeNull();
            sale.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void RemoveItems_Should_Clear_All_Items()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Select(i => new DeveloperEvaluation.Domain.Entities.SaleItem(i.Id, testData.Id, i.Product, i.Quantity, i.UnitPrice)).ToList());

            sale.RemoveItems();

            sale.Items.Should().BeEmpty();
        }

        [Fact]
        public void UpdateItems_Should_Add_And_Remove_Correct_Items()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            var initialItems = testData.Items.Take(2).Select(i => new DeveloperEvaluation.Domain.Entities.SaleItem(i.Id, testData.Id, i.Product, i.Quantity, i.UnitPrice)).ToList();
            sale.UpdateItems(initialItems);

            var newItem = new DeveloperEvaluation.Domain.Entities.SaleItem(Guid.NewGuid(), testData.Id, "Novo Produto", 5, 10m);
            var updatedItems = initialItems.Take(1).ToList();
            updatedItems.Add(newItem);

            sale.UpdateItems(updatedItems);

            sale.Items.Should().HaveCount(updatedItems.Count);
            sale.Items.Should().ContainSingle(i => i.Product == "Novo Produto");
            sale.Items.Should().NotContain(i => i.Product == initialItems[1].Product);
            sale.UpdatedAt.Should().NotBeNull();
        }
    }
}
