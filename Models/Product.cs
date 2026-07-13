using System.ComponentModel.DataAnnotations;

namespace GlassyStore.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string FrameType { get; set; }

        [Required]
        public string Material { get; set; }

        [Range(0, 10000)]
        public decimal Price { get; set; }

        [Range(0, 1000)]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}