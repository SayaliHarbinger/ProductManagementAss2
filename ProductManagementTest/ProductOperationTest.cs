using Microsoft.EntityFrameworkCore;
using Moq;
using ProductManagementAss2.Data;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.Domain;
using Xunit;

namespace ProuctOperationTest
{
    public class ProductOperationsTests
    {
        //private readonly ProductOperations _productOperations;
        //private readonly Mock<ProductDbContext> _productDbContextMock;

        //public ProductOperationsTests()
        //{
        //    _productDbContextMock = new Mock<ProductDbContext>(new DbContextOptions<ProductDbContext>()); 
        //    _productOperations = new ProductOperations(_productDbContextMock.Object);
        //}

        //[Fact]
        //public async Task AddProductAsync_ValidProduct_ReturnsAddedProduct()
        //{
        //    // Arrange
        //    var product = new Product { Name = "Test Product", Description = "Test Description", Category = "Test Category", Price = 10.99 };

        //    // Set up the behavior for DbSet (if needed)
        //    var productsDbSetMock = new Mock<DbSet<Product>>();
        //    _productDbContextMock.Setup(d => d.Products).Returns(productsDbSetMock.Object);

        //    // Act
        //    var addedProduct = await _productOperations.AddProductAsync(product);

        //    // Assert
        //    Assert.NotNull(addedProduct);
        //    Assert.Equal(product.Name, addedProduct.Name);
        //    Assert.Equal(product.Description, addedProduct.Description);
        //    Assert.Equal(product.Category, addedProduct.Category);
        //    Assert.Equal(product.Price, addedProduct.Price);
        //}
    }
}
