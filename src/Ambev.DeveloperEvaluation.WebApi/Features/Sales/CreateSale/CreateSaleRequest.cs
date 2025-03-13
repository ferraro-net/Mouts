namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
    public required string SaleNumber { get; set; }

    public required string Customer { get; set; }

    public required string Branch { get; set; }

    public DateTime SaleDate { get; set; }

    public List<CreateSaleItemRequest> Items { get; set; } = new List<CreateSaleItemRequest>();

}
public class CreateSaleItemRequest
{
    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}