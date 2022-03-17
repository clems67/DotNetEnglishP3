using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using P3AddNewFunctionalityDotNetCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Microsoft.Extensions.Configuration;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void TestUpdateStockAdminUser()
        {
            var provider = new ServiceCollection().AddEntityFrameworkSqlServer().BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<P3Referential>();
            builder.UseSqlServer($"Server=.\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true").UseInternalServiceProvider(provider);
            var context = new P3Referential(builder.Options);
            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            var MockCart = new Mock<Cart>();
            var MockOrderService = new Mock<IOrderService>();
            var MockLocaliser = new Mock<IStringLocalizer<OrderController>>();
            MockLocaliser.Setup(_ => _["CartEmpty"]).Returns(new LocalizedString("CartEmpty", "CartEmpty"));

            var MockOrderController = new OrderController(
                MockCart.Object,
                MockOrderService.Object,
                MockLocaliser.Object
                );

            var product = new Product
            {
                //Id = 1,
                Price = 35.1,
                Name = "new product",
                Description = "new product description",
                Details = "product details",
                Quantity = 3
            };
            var product2 = new Product
            {
                //Id = 2,
                Price = 44.4,
                Name = "new product2",
                Description = "new product2 description",
                Details = "product2 details",
                Quantity = 10
            };

            //ACT
            //create the SQL SERVER database and save the products inside of it
            var productService = new ProductRepository(context);
            productService.SaveProduct(product);
            productService.SaveProduct(product2);

            //insert "product" in an new OrderViewModel
            var OrderToTest = new OrderViewModel();
            OrderToTest.OrderId = 1;
            var CartLineNew = new CartLine();
            CartLineNew.OrderLineId = 1;
            CartLineNew.Product = product;
            CartLineNew.Quantity = 1;
            OrderToTest.Lines = new[] { CartLineNew };

            Assert.InRange(product.Quantity, 1, double.PositiveInfinity);
            MockOrderController.Index(OrderToTest);


        }

        [Fact]
        public void TestTempo()
        {
            var ProductControllerMock = new ProductControllerTest();
            var ProductRepository = new ProductRepository(ProductControllerMock.p3Referential);
            var product = new Product
            {
                //Id = 1,
                Price = 35.1,
                Name = "new product",
                Description = "new product description",
                Details = "product details",
                Quantity = 3
            };
            var product2 = new Product
            {
                //Id = 2,
                Price = 44.4,
                Name = "new product2",
                Description = "new product2 description",
                Details = "product2 details",
                Quantity = 10
            };

            ProductRepository.SaveProduct(product);
            ProductRepository.SaveProduct(product2);

            //MOCKING THE ORDER SERVICE
            var MockCart = new Mock<Cart>();
            var MockOrderRespository = new Mock<IOrderRepository>();
            var MockProductService = new Mock<IProductService>();
            var MockOrderService = new OrderService(
                MockCart.Object,
                MockOrderRespository.Object,
                MockProductService.Object
                );
            //MOCKING THE ORDER CONTROLLER
            //var MockOrderService = new Mock<IOrderService>();
            var MockLocaliser = new Mock<IStringLocalizer<OrderController>>();
            MockLocaliser.Setup(_ => _["CartEmpty"]).Returns(new LocalizedString("CartEmpty", "CartEmpty"));
            var MockOrderController = new OrderController(
                         MockCart.Object,
                         MockOrderService,
                         MockLocaliser.Object
                         );

            //insert "product" in an new OrderViewModel
            var OrderToTest = new OrderViewModel();
            OrderToTest.OrderId = 1;
            var CartLineNew = new CartLine();
            CartLineNew.OrderLineId = 1;
            CartLineNew.Product = product;
            CartLineNew.Quantity = 1;
            OrderToTest.Lines = new[] { CartLineNew };

            //Assert.InRange(product.Quantity, 1, double.PositiveInfinity);
            MockOrderController.Index(OrderToTest);


        }

        [Fact]
        public void TestTempo2()
        {
            //DataBaseTest MockDataBase = new DataBaseTest();
            //  Setting up the stuff required for Configuration.GetConnectionString("DefaultConnection")
            Mock<IConfigurationSection> configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("TestConnectionString");
            Mock<Microsoft.Extensions.Configuration.IConfiguration> configurationStub = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);
            configurationStub.Setup(x => x.GetConnectionString("P3Referential")).Returns("Server=.\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true");
            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);

            //Act

            target.ConfigureServices(services);
            //  Mimic internal asp.net core logic.
            services.AddTransient<OrderController>();
            services.AddTransient<ProductController>();

            //Assert

            var serviceProvider = services.BuildServiceProvider();

            OrderController MockOrderController = serviceProvider.GetService<OrderController>();
            ProductController MockProductController = serviceProvider.GetService<ProductController>();

            var product = new Product //used to create an OrderViewModel, so we can fill a Cart
            {
                //Id = 1,
                Price = 35.1,
                Name = "new product",
                Description = "new product description",
                Details = "product details",
                Quantity = 3
            };
            var product2 = new Product
            {
                //Id = 2,
                Price = 44.4,
                Name = "new product2",
                Description = "new product2 description",
                Details = "product2 details",
                Quantity = 10
            };
            //CREATION OF PRODUCTVIEWMODELS SO THEY CAN BE SAVED IN THE DATABASE
            var ProductToBeSaved = new ProductViewModel();
            ProductToBeSaved.Id = 1;
            ProductToBeSaved.Name = "new product";
            ProductToBeSaved.Description = "new product description";
            ProductToBeSaved.Details = "product details";
            ProductToBeSaved.Stock = "3";
            //MockProductController.Create(ProductToBeSaved);
            ProductToBeSaved = new ProductViewModel(); //product saved in the database
            ProductToBeSaved.Id = 2;
            ProductToBeSaved.Name = "new product2";
            ProductToBeSaved.Description = "new product2 description";
            ProductToBeSaved.Details = "product2 details";
            ProductToBeSaved.Stock = "10";
            //MockProductController.Create(ProductToBeSaved); //product2 saved in the database



            var OrderToTest = new OrderViewModel();
            OrderToTest.OrderId = 1;
            var CartLineNew = new CartLine();
            CartLineNew.OrderLineId = 1;
            CartLineNew.Product = product;
            CartLineNew.Quantity = 1;
            OrderToTest.Lines = new[] { CartLineNew };

            MockOrderController.Index(OrderToTest); //save our cart (from what i've seen, it also update the stock in the database without verifying if the articles are still in the database)

        }

    }

    //public interface ISomeInterface { }

    //public class SomeImplementation : ISomeInterface
    //{
    //    public void SomeMethod()
    //    {

    //    }
    //}

    //public class AnotherImplementation : ISomeInterface { }

    //public class SomeClass
    //{
    //    public ISomeInterface SomeInstance { get; set; }

    //    public SomeClass(ISomeInterface someInstance)
    //    {
    //        this.SomeInstance = someInstance;

    //        ((SomeImplementation)SomeInstance).SomeMethod();
    //    }
    //}
}
