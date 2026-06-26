using System.ComponentModel.DataAnnotations;

namespace SportEquipment.Mvc.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // Tương đương SKU
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int MinStock { get; set; }
        
        // Relationship
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}