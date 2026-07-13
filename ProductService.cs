using GlassyStore.Models;
using GlassyStore.Repositories;

namespace GlassyStore.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _repository.AddAsync(product);
            await _repository.SaveAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _repository.UpdateAsync(product);
            await _repository.SaveAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveAsync();
        }
    }
}