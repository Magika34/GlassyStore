using System.ComponentModel.DataAnnotations;

namespace GlassyStore.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int LensOptionId { get; set; }

        public LensOption? LensOption { get; set; }

        public decimal PrescriptionSphereLeft { get; set; }

        public decimal PrescriptionSphereRight { get; set; }

        public decimal PrescriptionCylinderLeft { get; set; }

        public decimal PrescriptionCylinderRight { get; set; }

        [Range(45, 80)]
        public int PupillaryDistance { get; set; }
    }
}