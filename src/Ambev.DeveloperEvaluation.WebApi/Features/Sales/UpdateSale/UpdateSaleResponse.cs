using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleResponse
{
    public Guid Id { get; set; }

    public required string SaleNumber { get; set; }

    public required string Customer { get; set; }

    public required string Branch { get; set; }

    public DateTime SaleDate { get; set; }

    public List<SaleItem> Items { get; private set; } = new List<SaleItem>();

    public SaleStatus Status { get; set; }
}
