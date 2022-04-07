using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using Xunit;


namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class CartUpdated
    {
        [Fact]
        public async void CartUpdateWhenItemIsDeletedFromDatabase()
        {
            //ARANGE
            Mock<IConfigurationSection> configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("TestConnectionString");
            Mock<Microsoft.Extensions.Configuration.IConfiguration> configurationStub = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);
            configurationSectionStub.Setup(x => x["P3Referential"])
                .Returns("Server=.\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true");
            //configurationStub.Setup(x => x.GetConnectionString("P3Referential")).Returns("Server=.\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true");
            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);
            var serviceProvider = services.BuildServiceProvider();
            target.ConfigureServices(services);

            var product = new Product //used to create an OrderViewModel, so we can fill a Cart
            {
                //Id = 1,
                Name = "new product",
                Description = "new product description",
                Details = "product details",
                Quantity = 3,
                Price = 35.1,
            };
            var product2 = new Product
            {
                //Id = 2,
                Name = "new product2",
                Description = "new product2 description",
                Details = "product2 details",
                Quantity = 10,
                Price = 44.4,
            };

            //CREATION OF PRODUCTVIEWMODELS SO THEY CAN BE SAVED IN THE DATABASE
            var ProductToBeSaved1 = new ProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Details = product.Details,
                Stock = product.Quantity.ToString(),
                Price = product.Price.ToString(),
            };
            var ProductToBeSaved2 = new ProductViewModel
            {
                Name = product2.Name,
                Description = product2.Description,
                Details = product2.Details,
                Stock = product2.Quantity.ToString(),
                Price = product2.Price.ToString(),
            };

            services.AddTransient<OrderController>();
            services.AddTransient<ProductController>();
            services.AddTransient<CartController>();
            services.AddTransient<ProductService>();
            services.AddTransient<OrderRepository>();

            serviceProvider = services.BuildServiceProvider();

            OrderController MockOrderController = serviceProvider.GetService<OrderController>();
            ProductController MockProductController = serviceProvider.GetService<ProductController>();
            CartController MockCartController = serviceProvider.GetService<CartController>();
            ProductService MockProductService = serviceProvider.GetService<ProductService>();
            OrderRepository MockOrderRepository = serviceProvider.GetRequiredService<OrderRepository>();

            //ACT
            MockProductController.Create(ProductToBeSaved1);
            MockProductController.Create(ProductToBeSaved2);

            product = FindID(MockProductService, ProductToBeSaved1);
            product2 = FindID(MockProductService, ProductToBeSaved2);

            MockCartController.AddToCart(product.Id);
            MockCartController.AddToCart(product2.Id);

            MockProductController.DeleteProduct(product.Id);

            var previousOrders = await MockOrderRepository.GetOrders();
            List<int> ListIdPreviousOrders = new List<int>();
            foreach (var order in previousOrders) { ListIdPreviousOrders.Add(order.Id); }

            MockOrderController.Index(new OrderViewModel()); //save our cart (from what i've seen, it also update the stock in the database without verifying if the articles are still in the database)

            //ASSERT
            var NextOrders = await MockOrderRepository.GetOrders();
            int OrderId = 0;
            foreach (var order in NextOrders)
            {
                if (!ListIdPreviousOrders.Contains(order.Id))
                { OrderId = order.Id; }
            }
            var OrderToTest = await MockOrderRepository.GetOrder(OrderId);

            Assert.Single(OrderToTest.OrderLine);
        }

        private Product FindID(ProductService productService, ProductViewModel product)
        {
            return productService.GetAllProducts()
                .Find(p => p.Name == product.Name
                        && p.Description == product.Description
                        && p.Details == product.Details
                    );
        }
    }
}
