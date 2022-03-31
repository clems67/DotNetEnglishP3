using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using Moq;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

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
        public async void TestTempo2()
        {
            //DataBaseTest MockDataBase = new DataBaseTest();
            //  Setting up the stuff required for Configuration.GetConnectionString("DefaultConnection")
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
            //Act


            //Assert
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

            var NextOrders = await MockOrderRepository.GetOrders();

            int OrderId = 0;
            foreach (var order in NextOrders)
            {
                if (!ListIdPreviousOrders.Contains(order.Id)) { OrderId = order.Id; }
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
