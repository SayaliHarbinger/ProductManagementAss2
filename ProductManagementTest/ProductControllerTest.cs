using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementAss2.Controllers;
using ProductManagementAss2.Data;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.Domain;
using ProductManagementAss2.Models.DTO;
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

        [Theory]
        [InlineData("name", "description", "category", 12)]
        public async Task Product_Created_for_valid_data(string name, string description, string category, double price)
        {
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);

            var validModel = new Product()
            {
                Name = name,
                Description = description,
                Category = category,
                Price = price,
            };

         

            // Act
            var result = await controller.Create(validModel) as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }
        [Fact]
        public async Task Product_Created_for_Invalid_data()
        {
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);
            var invalidModel = new Product();

            controller.ModelState.AddModelError("Price", "Price is required");

            // Act
            var result = await controller.Create(invalidModel);

            // Assert
            Assert.IsType<ViewResult>(result);
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

            mockDb.Setup(db => db.GetProductAsync(invalidProductId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await controller.Update(invalidProductId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task DeleteConfirmed_ValidId_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockDb = new Mock<IProductOperations>();
            var controller = new ProductController(mockDb.Object);
            var productId = Guid.NewGuid();
            var existingProduct = new Product
            {
                ProdId = productId,
                Name = "Samsung",
                Description = "Its a mobile",
                Price = 12990
            };

            mockDb.Setup(db => db.GetProductAsync(productId))
                .ReturnsAsync(existingProduct);

            // Act
            var result = await controller.DeleteConfirmed(productId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }
        [Fact]
        public async Task DeleteConfirmed_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockDb = new Mock<IProductOperations>();
            var controller = new ProductController(mockDb.Object);
            var invalidProductId = Guid.NewGuid();

            mockDb.Setup(db => db.GetProductAsync(invalidProductId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await controller.DeleteConfirmed(invalidProductId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

    }
}
