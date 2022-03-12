using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using P3AddNewFunctionalityDotNetCore.Data;
using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

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
            OrderToTest.Lines = new [] {CartLineNew};

            Assert.InRange(product.Quantity, 1, double.PositiveInfinity);
            MockOrderController.Index(OrderToTest);


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
