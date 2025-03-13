using Xunit;
using FluentAssertions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.SaleItem.TestData;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.SaleItem
{
    public class SaleItemTests
    {
        [Fact]
        public void SaleItem_Should_Initialize_Correctly()
        {
            var testData = new SaleItemTestData();

            var saleItem = new DeveloperEvaluation.Domain.Entities.SaleItem(testData.Id, testData.SaleId, testData.Product, testData.Quantity, testData.UnitPrice);

            saleItem.Should().NotBeNull();
            saleItem.Id.Should().Be(testData.Id);
            saleItem.SaleId.Should().Be(testData.SaleId);
            saleItem.Product.Should().Be(testData.Product);
            saleItem.Quantity.Should().Be(testData.Quantity);
            saleItem.UnitPrice.Should().Be(testData.UnitPrice);
            saleItem.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(3, 50, 0.00, 150)]
        [InlineData(5, 50, 0.10, 225)]
        [InlineData(10, 50, 0.20, 400)]
        public void CalculateDiscountAndTotal_Should_Return_Correct_Discount_And_Total(int quantity, decimal unitPrice, decimal expectedDiscount, decimal expectedTotal)
        {
            var (discount, totalAmount) = DeveloperEvaluation.Domain.Entities.SaleItem.CalculateDiscountAndTotal(quantity, unitPrice);

            discount.Should().Be(expectedDiscount);
            totalAmount.Should().BeApproximately(expectedTotal, 0.01m);
        }

        [Fact]
        public void CalculateTotals_Should_Update_Properties_Correctly()
        {
            var testData = new SaleItemTestData();

            var saleItem = new DeveloperEvaluation.Domain.Entities.SaleItem(testData.Id, testData.SaleId, testData.Product, testData.Quantity, testData.UnitPrice);
            var (expectedDiscount, expectedTotalAmount) = DeveloperEvaluation.Domain.Entities.SaleItem.CalculateDiscountAndTotal(testData.Quantity, testData.UnitPrice);

            saleItem.Discount.Should().Be(expectedDiscount);
            saleItem.TotalAmount.Should().BeApproximately(expectedTotalAmount, 0.01m);
        }
    }
}
