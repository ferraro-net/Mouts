namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequest
{
    public Guid Id { get; set; }

    public required string SaleNumber { get; set; }

    public required string Customer { get; set; }

    public required string Branch { get; set; }

    public DateTime SaleDate { get; set; }

    public List<UpdateSaleItemRequest> Items { get; set; } = new List<UpdateSaleItemRequest>();

}
public class UpdateSaleItemRequest
{
    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}