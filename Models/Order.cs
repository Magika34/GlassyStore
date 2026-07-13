using System.ComponentModel.DataAnnotations;

namespace GlassyStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}