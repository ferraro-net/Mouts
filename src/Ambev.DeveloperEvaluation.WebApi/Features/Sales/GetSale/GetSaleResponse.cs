namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

public class GetSaleResponse
{
    public Guid Id { get; set; }

    public required string SaleNumber { get; set; }

    public required string Customer { get; set; }

    public required string Branch { get; set; }

    public DateTime SaleDate { get; set; }

    public List<SaleItemResponse> Items { get; set; } = new List<SaleItemResponse>();

}

public class SaleItemResponse
{
    public Guid Id { get; set; }

    public required string Product { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal TotalAmount { get; set; }
}
