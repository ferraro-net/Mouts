using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }

    public Sale? Sale {  get; private set; }

    public string Product { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Discount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public SaleItem() { }

    public SaleItem(Guid id, Guid saleId, string product, int quantity, decimal unitPrice)
    {
        Id = id;
        SaleId = saleId;
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CreatedAt = DateTime.UtcNow;

        CalculateTotals(quantity, unitPrice);
    }

    public void UpdateItem(int quantity, decimal unitPrice)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        CalculateTotals(quantity, unitPrice);
    }

    public void CalculateTotals(int quantity, decimal unitPrice)
    {
        (Discount, TotalAmount) = CalculateDiscountAndTotal(quantity, unitPrice);
    }

    public static (decimal discount, decimal totalAmount) CalculateDiscountAndTotal(int quantity, decimal unitPrice)
    {
        decimal discount = (quantity >= 10) ? 0.20m : (quantity > 4) ? 0.10m : 0m;
        decimal totalAmount = (unitPrice * quantity) * (1 - discount);

        return (discount, totalAmount);
    }
}