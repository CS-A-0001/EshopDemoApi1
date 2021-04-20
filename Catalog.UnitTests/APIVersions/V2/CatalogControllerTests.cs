using AutoMapper;
using CatalogDemo.API.Infrastructure;
using CatalogDemo.API.Model;
using CatalogDemo.API.Versions.V2.AutomapperProfiles;
using CatalogDemo.API.Versions.V2.Controllers;
using CatalogDemo.API.Versions.V2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests.APIVersions.V2
{    
    public class CatalogControllerTests
    {        
        readonly IMapper mapper;
        readonly string VersionPrefix = "V2";
        public CatalogControllerTests()
        {            

            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new ProductProfile());
            });
            mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Put_catalog_item_description_success()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(databaseName: $"{VersionPrefix}{nameof(Put_catalog_item_description_success)}")
                .Options;

            using var dbContext = new CatalogContext(dbOptions);
            dbContext.AddRange(GetFakeCatalog(2));
            dbContext.SaveChanges();
            var newDescription = "Updated description";

            //Act
            var orderController = new CatalogController(dbContext, mapper);
            var actionResult = await orderController.UpdateProductAsync(2, newDescription);            

            //Assert
            Assert.IsType<NoContentResult>(actionResult);
            Assert.Equal(newDescription, dbContext.Products.FirstOrDefault(product => product.Id == 2).Description);
        }


        [Fact]
        public async Task Put_catalog_item_description_notfound()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(databaseName: $"{VersionPrefix}{nameof(Put_catalog_item_description_notfound)}")
                .Options;

            using var dbContext = new CatalogContext(dbOptions);
            dbContext.AddRange(GetFakeCatalog(2));
            dbContext.SaveChanges();
            var newDescription = "Updated description";

            //Act
            var orderController = new CatalogController(dbContext, mapper);
            var actionResult = await orderController.UpdateProductAsync(3, newDescription);

            //Assert
            Assert.IsType<NotFoundObjectResult>(actionResult);            
        }

        [Fact]
        public async Task Get_catalog_item_success()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(databaseName: $"{VersionPrefix}{nameof(Get_catalog_item_success)}")
                .Options;

            using var dbContext = new CatalogContext(dbOptions);            
            dbContext.AddRange(GetFakeCatalog(2));
            dbContext.SaveChanges();            

            //Act
            var orderController = new CatalogController(dbContext, mapper);
            var actionResult = await orderController.ItemByIdAsync(2);

            //Assert
            Assert.IsType<ActionResult<ProductDTO>>(actionResult);
            var result = Assert.IsAssignableFrom<ProductDTO>(actionResult.Value);
            var dbItem = GetFakeCatalog(2).ElementAt(1);            
            Assert.Equal(dbItem.Name, result.Name);
            Assert.Equal(dbItem.Description, result.Description);
            Assert.Equal(dbItem.ImgUri, result.ImgUri);
            Assert.Equal(dbItem.Price, result.Price);
        }

        [Fact]
        public async Task Get_catalog_item_notfound()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(databaseName: $"{VersionPrefix}{nameof(Get_catalog_item_notfound)}")
                .Options;

            using var dbContext = new CatalogContext(dbOptions);
            dbContext.AddRange(GetFakeCatalog(1));
            dbContext.SaveChanges();            

            //Act
            var orderController = new CatalogController(dbContext, mapper);
            var actionResult = await orderController.ItemByIdAsync(2);

            //Assert            
            Assert.IsType<NotFoundResult>(actionResult.Result);            
        }

        [Fact]
        public async Task Get_catalog_item_badrequest()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(databaseName: $"{VersionPrefix}{nameof(Get_catalog_item_badrequest)}")
                .Options;

            using var dbContext = new CatalogContext(dbOptions);
            dbContext.AddRange(GetFakeCatalog(1));
            dbContext.SaveChanges();

            //Act
            var orderController = new CatalogController(dbContext, mapper);
            var actionResult = await orderController.ItemByIdAsync(0);

            //Assert            
            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task Get_catalog_items_default_success()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
              .UseInMemoryDatabase(databaseName: $"{VersionPrefix}{nameof(Get_catalog_items_default_success)}")
              .Options;

            using var dbContext = new CatalogContext(dbOptions);
            dbContext.AddRange(GetFakeCatalog(20));
            dbContext.SaveChanges();
           
            //Act
            var orderController = new CatalogController(dbContext, mapper);
            var actionResult = await orderController.ItemsAsync();

            //Assert
            
            Assert.IsType<OkObjectResult>(actionResult);
            var page = Assert.IsAssignableFrom<PaginatedItems<ProductDTO>>((actionResult as OkObjectResult).Value);
            Assert.Equal(20, page.Count);
            Assert.Equal(0, page.PageIndex);
            Assert.Equal(10, page.PageSize);
            Assert.Equal(10, page.Data.Count());            
        }

        [Fact]
        public async Task Get_catalog_items_custom_page_success()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
              .UseInMemoryDatabase(databaseName: $"{VersionPrefix}{nameof(Get_catalog_items_custom_page_success)}")
              .Options;

            using var dbContext = new CatalogContext(dbOptions);
            dbContext.AddRange(GetFakeCatalog(20));
            dbContext.SaveChanges();

            //Act
            var orderController = new CatalogController(dbContext, mapper);
            var actionResult = await orderController.ItemsAsync(5,1);

            //Assert

            Assert.IsType<OkObjectResult>(actionResult);
            var page = Assert.IsAssignableFrom<PaginatedItems<ProductDTO>>((actionResult as OkObjectResult).Value);
            Assert.Equal(20, page.Count);
            Assert.Equal(1, page.PageIndex);
            Assert.Equal(5, page.PageSize);
            Assert.Equal(5, page.Data.Count());
        }

        static IEnumerable<Product> GetFakeCatalog(int size)
        {
            var genericProduct = new Product { Name = "Product", ImgUri = @"Url", Price = 1M, Description = "Description" };

            var result = new List<Product>();

            for(int i = 0; i < size; i++)
            {
                result.Add(new Product { Name = $"{genericProduct.Name}{i}", ImgUri = $"{genericProduct.Name}{i}", Price = genericProduct.Price + i, Description = $"{genericProduct.Description}{i}" });
            }
            return result;
        }
    }
}
