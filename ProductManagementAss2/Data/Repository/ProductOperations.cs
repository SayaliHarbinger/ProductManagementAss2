using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementAss2.Models.Domain;
//using System.Web.Mvc;

namespace ProductManagementAss2.Data.Repository
{
    public class ProductOperations: IProductOperations
    {
        private readonly ProductDbContext _dbcontext;
        public ProductOperations(ProductDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

       
        public async  Task<Product> AddProductAsync(Product product)
        {
            _dbcontext.Products.Add(product);
            await _dbcontext.SaveChangesAsync();
           
            return product;
        }

    
        public async Task DeleteProductAsync(Guid Id)
        {
           
            var product = await _dbcontext.Products.FirstOrDefaultAsync(p => p.ProdId == Id);
            
            if (product != null)
            {

                _dbcontext.Remove(product); 
                await _dbcontext.SaveChangesAsync(); 
            }

          
        }

        public async Task<Product> GetProductAsync(Guid Id)
        {
            var product=await _dbcontext.Products.FirstOrDefaultAsync(p=>p.ProdId == Id);
            return product;
        }
        public async Task<List<Product>> GetProductListAsync()
        {
            return _dbcontext.Products.ToList();
        }

        public async Task<Product> UpdateProductAsync(Guid Id, Product product)
        {

            var existingProduct = await _dbcontext.Products.FindAsync(Id);
            if (existingProduct == null)
            {
                throw new Exception("Product Not Found");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;

            await _dbcontext.SaveChangesAsync();

            return existingProduct;
        }
    }
}
