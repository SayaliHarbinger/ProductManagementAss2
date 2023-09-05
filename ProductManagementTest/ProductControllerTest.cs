using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementAss2.Controllers;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.Domain;
using ProductManagementAss2.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementTest
{
    public class ProductControllerTest
    {

        private Mock<IProductOperations> CreateMockProductOperations()
        {
            return new Mock<IProductOperations>();
        }
        [Theory]
        [InlineData("name", "description", "category", 12)]
        public async Task Product_Created_for_valid_data(string name, string description, string category, double price)
        {
            var mockProductOperations = CreateMockProductOperations();
            var controller = new ProductController(mockProductOperations.Object);

            var validModel = new Product();

            validModel.Name = name;
            validModel.Description = description;
            validModel.Category = category;
            validModel.Price = price;

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

            // Act
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

            // Act
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

    }
}
