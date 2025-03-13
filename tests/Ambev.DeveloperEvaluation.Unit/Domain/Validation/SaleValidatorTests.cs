using Xunit;
using FluentValidation.TestHelper;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.Sales.Sale.TestData;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation
{
    public class SaleValidatorTests
    {
        private readonly SaleValidator _validator;

        public SaleValidatorTests()
        {
            _validator = new SaleValidator();
        }

        [Fact]
        public void Validate_Should_Pass_For_Valid_Sale()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Select(f=> new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, f.Quantity, f.UnitPrice)).ToList());
            var result = _validator.TestValidate(sale);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_Should_Fail_When_SaleNumber_Is_Empty()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, string.Empty, testData.Customer, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Select(f => new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, f.Quantity, f.UnitPrice)).ToList());
            var result = _validator.TestValidate(sale);

            result.ShouldHaveValidationErrorFor(s => s.SaleNumber);
        }

        [Fact]
        public void Validate_Should_Fail_When_Customer_Is_Empty()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, string.Empty, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Select(f => new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, f.Quantity, f.UnitPrice)).ToList());
            var result = _validator.TestValidate(sale);

            result.ShouldHaveValidationErrorFor(s => s.Customer);
        }

        [Fact]
        public void Validate_Should_Fail_When_Branch_Is_Empty()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, string.Empty, testData.SaleDate);
            sale.UpdateItems(testData.Items.Select(f => new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, f.Quantity, f.UnitPrice)).ToList());
            var result = _validator.TestValidate(sale);

            result.ShouldHaveValidationErrorFor(s => s.Branch);
        }

        [Fact]
        public void Validate_Should_Fail_When_SaleDate_Is_Default()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, default);
            sale.UpdateItems(testData.Items.Select(f => new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, f.Quantity, f.UnitPrice)).ToList());
            var result = _validator.TestValidate(sale);

            result.ShouldHaveValidationErrorFor(s => s.SaleDate);
        }

        [Fact]
        public void Validate_Should_Fail_When_Items_List_Is_Empty()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Select(f => new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, f.Quantity, f.UnitPrice)).ToList());
            sale.RemoveItems();

            var result = _validator.TestValidate(sale);

            result.ShouldHaveValidationErrorFor(s => s.Items);
        }

        [Fact]
        public void Validate_Should_Fail_When_An_Item_Has_Quantity_Zero_Or_Less()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Take(1).Select(f => new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, 0, f.UnitPrice)).ToList());
            var result = _validator.TestValidate(sale);

            result.ShouldHaveValidationErrorFor("Items[0].Quantity");
        }

        [Fact]
        public void Validate_Should_Fail_When_An_Item_Has_Quantity_Greater_Than_20()
        {
            var testData = new SaleTestData();
            var sale = new DeveloperEvaluation.Domain.Entities.Sale(testData.Id, testData.SaleNumber, testData.Customer, testData.Branch, testData.SaleDate);
            sale.UpdateItems(testData.Items.Take(1).Select(f => new DeveloperEvaluation.Domain.Entities.SaleItem(f.Id, testData.Id, f.Product, 21, f.UnitPrice)).ToList());
            var result = _validator.TestValidate(sale);

            result.ShouldHaveValidationErrorFor("Items[0].Quantity");
        }
    }
}
