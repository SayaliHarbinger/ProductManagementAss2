using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementAss2.Controllers;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.Domain;
using System.Security.Claims;

namespace ProductManagementTest
{
    public class ProductControllerTest
    {

        private static Mock<IProductOperations> CreateMockProductOperations()
        {
            return new Mock<IProductOperations>();
        }
        [Fact]
        public void Create_ReturnsViewResult()
        {
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);

            // Act
            var result = controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        //Add valid Product
        [Fact]
        public async Task Add_Valid_ProductAsync()
        {
            // Arrange
            var mockAdminService = new Mock<IProductOperations>();
            var controller = new ProductController(mockAdminService.Object);
            var validProduct = new Product
            {
                Name = "Sample Book",
                Description = "John Doe",
                Category = "Description",
                Price = 1234
            };

            // Setup the service to return a valid product
            mockAdminService.Setup(service => service.AddProductAsync(It.IsAny<Product>()))
                .ReturnsAsync(new Product { ProdId = Guid.NewGuid() }); // Assuming AddProductAsync returns a valid product

            // Act
            var result = await controller.Create(validProduct) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(controller.ModelState.IsValid, "ModelState should be valid.");
            mockAdminService.Verify();
        }
        [Fact]
        public async Task Add_Invalid_ProductAsync()
        {
            // Arrange
            var mockAdminService = new Mock<IProductOperations>();
            var controller = new ProductController(mockAdminService.Object);
            var invalidProduct = new Product
            {
                Name = string.Empty, // An invalid product with an empty name
                Description = "John Doe",
                Category = "Description",
                Price = 1234
            };

            // Setup the service to return an invalid product
            mockAdminService.Setup(service => service.AddProductAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await controller.Create(invalidProduct) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains(controller.ModelState.Values, v => v.Errors.Count > 0);
            Assert.Equal("Name", result.ViewData.ModelState.Keys.First()); // Adjust this key as per your actual validation logic
        }


        [Fact]
        public async Task Index_ValidData_ReturnsView()
        {
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);

            
            var expectedProducts = new List<Product>
             {
                 new Product { ProdId = Guid.NewGuid(), Name = "Product1" ,Description="Description",Category="Mobile",Price=12},
                    new Product { ProdId = Guid.NewGuid(), Name = "Product2" ,Description="Description",Category="Mobile",Price=12 }
                };
            mockProductOperations.Setup(db => db.GetProductListAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProducts, result.Model); 
        }
        [Fact]
        public async Task Index_InvalidData_ReturnsView()
        {
           
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);

            
            mockProductOperations.Setup(db => db.GetProductListAsync())
                .ReturnsAsync(new List<Product>());
            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Detail_ReturnsViewResult_ForUser()
        {
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, "username"),
        new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = controller.Detail() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Detail_ValidId_ReturnsViewWithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);


            var expectedProduct = new Product { ProdId = productId, Name = "TestProduct" };
            mockProductOperations.Setup(db => db.GetProductAsync(productId))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await controller.Detail(productId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct, result.Model); 
        }
        [Fact]
        public async Task Detail_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidProductId = Guid.NewGuid();
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);

            mockProductOperations.Setup(db => db.GetProductAsync(invalidProductId))
                .ReturnsAsync((Product)null);

            var result = await controller.Detail(invalidProductId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Valid_RedirectsToActionIndex()
        {
            var productId = Guid.NewGuid();
            var mockProductOperations = CreateMockProductOperations();
            var controller= new ProductController(mockProductOperations.Object);
            var validProduct = new Product { ProdId = productId, Name = "UpdatedProduct" };
            mockProductOperations.Setup(db => db.GetProductAsync(productId))
                .ReturnsAsync(validProduct);

            // Act
            var result = await controller.Update(productId, validProduct) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }
        [Fact]
        public async Task Update_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var productId = Guid.NewGuid(); 
            var mockProductOperations = new Mock<IProductOperations>();
            var controller = new ProductController(mockProductOperations.Object);
            var invalidProduct = new Product { ProdId = Guid.NewGuid(), Name = "InvalidProduct" }; 

           
       mockProductOperations.Setup(db => db.GetProductAsync(productId)).ReturnsAsync((Product)null);

            var result = await controller.Update(productId, invalidProduct) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task Delete_ValidId_ReturnsViewWithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid(); 
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);

          
            var expectedProduct = new Product { ProdId = productId, Name = "TestProduct" };
            mockProductOperations.Setup(db => db.GetProductAsync(productId))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await controller.Delete(productId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct, result.Model); 
        }
        [Fact]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidProductId = Guid.NewGuid(); 
            var mockProductOperations = new Mock<IProductOperations>();
            var controller = new ProductController(mockProductOperations.Object);
            mockProductOperations.Setup(db => db.GetProductAsync(invalidProductId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await controller.Delete(invalidProductId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Update_ValidId_ReturnsViewResult()
        {
            // Arrange
            var mockDb = new Mock<IProductOperations>();
            var controller = new ProductController(mockDb.Object);
            var productId = Guid.NewGuid();
            var existingProduct = new Product
            {
                ProdId = productId,
                Name="Samsung",
                Description="Its a mobile",
                Price=12990
            };

            mockDb.Setup(db => db.GetProductAsync(productId))
                .ReturnsAsync(existingProduct);

            // Act
            var result = await controller.Update(productId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingProduct, result.Model);
        }

        [Fact]
        public async Task Update_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockDb = new Mock<IProductOperations>();
            var controller = new ProductController(mockDb.Object);
            var invalidProductId = Guid.NewGuid();
            var existingProduct = new Product
            {
                ProdId = invalidProductId,
                Name = "Samsung",
                Description = "Its a mobile",
                Price = 12990
            };
            mockDb.Setup(db => db.GetProductAsync(invalidProductId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await controller.Update(invalidProductId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }
       
      
        [Fact]
        public async Task Update_ValidInput_RedirectsToIndex()
        {
            // Arrange
            var mockDb = new Mock<IProductOperations>();
            var controller = new ProductController(mockDb.Object);
            var id = Guid.NewGuid();
            var product = new Product { ProdId = id, Name = "Sample Product", Description = "Description", Category = "Category", Price = 10.0 };

            // Act
            var result = await controller.Update(id, product) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            mockDb.Verify(db => db.UpdateProductAsync(id, product), Times.Once);
        }

        //Edit Invalid product
        [Fact]
        public async Task Update_Invalid_Product()
        {
            // Arrange
            var mockAdminService = new Mock<IProductOperations>();
            var controller = new ProductController(mockAdminService.Object);
            var id = Guid.NewGuid();
            var product = new Product
            {
                ProdId = id, 
                Name = string.Empty,
                Description = "John Doe",
                Price = 2022,
                Category = "product" 
            };

            mockAdminService.Setup(service => service.UpdateProductAsync(id,product))
                .Throws<ArgumentException>();

            var result = await controller.Update(id, product) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains(controller.ModelState.Values, v => v.Errors.Count > 0);
            Assert.Equal("Title", result.ViewData.ModelState.Keys.First()); 
        }
       



        //Delete Invalid Product
        [Fact]
        public async Task DeleteConfirmed_Invalid_Product_ReturnsNotFound()
        {
            // Arrange
            var mockAdminService = new Mock<IProductOperations>();
            var controller = new ProductController(mockAdminService.Object);
            var productId = Guid.NewGuid(); 
            mockAdminService.Setup(service => service.DeleteProductAsync(productId))
                .Verifiable();

            var result = await controller.DeleteConfirmed(productId) as NotFoundResult;
            Assert.NotNull(result);
        }
        [Fact]
        public async Task DeleteConfirmed_ProductFound_RedirectsToIndex()
        {
            // Arrange
            var mockDb = new Mock<IProductOperations>();
            mockDb.Setup(db => db.GetProductAsync(It.IsAny<Guid>())).ReturnsAsync(new Product());
            var controller = new ProductController(mockDb.Object);
            var id = Guid.NewGuid();

            // Act
            var result = await controller.DeleteConfirmed(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            mockDb.Verify(db => db.DeleteProductAsync(id), Times.Once); 
        }

        [Fact]
        public async Task DeleteConfirmed_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockDb = new Mock<IProductOperations>();
            mockDb.Setup(db => db.GetProductAsync(It.IsAny<Guid>())).ReturnsAsync((Product)null);
            var controller = new ProductController(mockDb.Object);
            var id = Guid.NewGuid();

            // Act
            var result = await controller.DeleteConfirmed(id) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }


    }
}
