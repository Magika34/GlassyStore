using System.ComponentModel.DataAnnotations;
namespace GlassyStore.ViewModels 
{ public class ProductViewModel
    { 
        public int ProductId { get; set; } 

        [Required] 
        public string Name { get; set; } 
        public string Brand { get; set; } 
        public string FrameType { get; set; } 
        public string Material { get; set; } 
        public decimal Price { get; set; } 
        public int StockQuantity { get; set; } 
        public string? ImageUrl { get; set; }
    } 
}