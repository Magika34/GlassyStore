using GlassyStore.Models;
using GlassyStore.Repositories;

namespace GlassyStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<LensOption> _lensRepository;

        public OrderService(
            IRepository<Product> productRepository,
            IRepository<LensOption> lensRepository)
        {
            _productRepository = productRepository;
            _lensRepository = lensRepository;
        }

        public async Task<decimal> CalculateTotalAsync(OrderItem orderItem)
        {
            var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
            var lens = await _lensRepository.GetByIdAsync(orderItem.LensOptionId);

            if (product == null || lens == null)
                return 0;

            return product.Price + lens.PriceModifier;
        }

        public async Task<bool> IsInStockAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            return product != null && product.StockQuantity > 0;
        }

        public async Task ReduceStockAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product != null)
            {
                product.StockQuantity--;

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveAsync();
            }
        }
    }
}