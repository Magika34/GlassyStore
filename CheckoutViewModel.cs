using System.ComponentModel.DataAnnotations;

namespace GlassyStore.ViewModels
{
    public class CheckoutViewModel
    {
        // Selected Frame
        [Required]
        public int ProductId { get; set; }

        // Selected Lens
        [Required]
        public int LensOptionId { get; set; }

        [Range(-20, 20)]
        public decimal PrescriptionSphereLeft { get; set; }

        [Range(-20, 20)]
        public decimal PrescriptionSphereRight { get; set; }

        [Range(-6, 6)]
        public decimal PrescriptionCylinderLeft { get; set; }

        [Range(-6, 6)]
        public decimal PrescriptionCylinderRight { get; set; }

        [Range(45, 80)]
        public int PupillaryDistance { get; set; }

        public decimal TotalPrice { get; set; }
    }
}