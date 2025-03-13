using Ambev.DeveloperEvaluation.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.Persistence.MongoDB.DTO
{
    public class SaleLog
    {
        [BsonId]
        public string Id { get; set; }

        public required string SaleNumber { get; set; }

        public DateTime SaleDate { get; set; }

        public required string Customer { get; set; }

        public decimal TotalSaleAmount { get; set; }

        public required string Branch { get; set; }

        public List<SaleItemLog> Items { get; set; } = new List<SaleItemLog>();

        public SaleStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class SaleItemLog
    {
        public string Id { get; set; }

        public required string Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
