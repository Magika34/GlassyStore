using System.ComponentModel.DataAnnotations;

namespace GlassyStore.Models
{
    public class LensOption
    {
        public int LensOptionId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Material { get; set; }

        public string? Coating { get; set; }

        [Range(0, 1000)]
        public decimal PriceModifier { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}