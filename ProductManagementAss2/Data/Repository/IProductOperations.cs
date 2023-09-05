using ProductManagementAss2.Models.Domain;

namespace ProductManagementAss2.Data.Repository
{
    public interface IProductOperations
    {
        public Task<List<Product>> GetProductListAsync();
        public Task<Product> GetProductAsync(Guid Id);
        public Task<Product> AddProductAsync(Product product);
        public Task<Product> UpdateProductAsync(Guid Id,Product product);
        public Task DeleteProductAsync(Guid Id);
    }
}
