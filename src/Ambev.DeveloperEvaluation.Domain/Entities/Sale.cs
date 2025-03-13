using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; private set; }

    public DateTime SaleDate { get; private set; }

    public string Customer { get; private set; }

    public decimal TotalSaleAmount => Items.Sum(i => i.TotalAmount);

    public string Branch { get; private set; }

    public List<SaleItem> Items { get; private set; } = new List<SaleItem>();
     
    public SaleStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public Sale(Guid id, string saleNumber, string customer, string branch, DateTime saleDate)
    {
        Id = id;
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        Customer = customer;
        Branch = branch;
        CreatedAt = DateTime.UtcNow;
        Status = SaleStatus.Active;
    }

    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    public void RemoveItems()
    {
        Items.Clear();
    }

    public void UpdateItems(List<SaleItem> items)
    {
        var itemsToRemove = Items.Where(i => !items.Any(newItem => newItem.Id == i.Id)).ToList();
        var itemsToAdd = items.Where(newItem => !Items.Any(i => i.Id == newItem.Id)).ToList();

        foreach (var item in itemsToRemove)
        {
            Items.Remove(item);
        }

        foreach (var item in itemsToAdd)
        {
            Items.Add(item);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCustomer(string customer)
    {
        Customer = customer;
        UpdatedAt = DateTime.UtcNow;
    }
}