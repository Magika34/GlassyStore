using GlassyStore.Models;

namespace GlassyStore.Services
{
    public interface IOrderService
    {
        Task<decimal> CalculateTotalAsync(OrderItem orderItem);

        Task<bool> IsInStockAsync(int productId);

        Task ReduceStockAsync(int productId);
    }
}