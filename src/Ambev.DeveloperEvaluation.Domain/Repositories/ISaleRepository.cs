using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateAsync(Sale Sale, CancellationToken cancellationToken = default);

    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Sale?> GetBySaleNumberAsync(string id, string branch, CancellationToken cancellationToken = default);

    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SaleItem?> GetSaleItemByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SaleItem> CreateSaleItemAsync(SaleItem saleItem, CancellationToken cancellationToken = default);

    Task UpdateSaleItemAsync(SaleItem saleItem, CancellationToken cancellationToken = default);

    Task DeleteSaleItemsAsync(List<SaleItem> saleItems, CancellationToken cancellationToken = default);
}
