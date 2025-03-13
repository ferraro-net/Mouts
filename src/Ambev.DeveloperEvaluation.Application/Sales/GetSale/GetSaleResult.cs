
namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleResult
{
    public Guid Id { get; set; }

    public required string SaleNumber { get; set; }

    public DateTime SaleDate { get; set; }

    public required string Customer { get; set; }

    public decimal TotalSaleAmount { get; set; }

    public required string Branch { get; set; }

    public List<SaleItemResult> Items { get; set; } = new List<SaleItemResult>();

    public required string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class SaleItemResult
{
    public Guid Id { get; set; }

    public required string Product { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal TotalAmount { get; set; }
}
