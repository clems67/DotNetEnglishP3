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

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async void CartUpdateWhenItemIsDeletedFromDatabase()
        {
            //ARANGE
            var database = new DataBaseSetup();
            var services = database.services;
            var serviceProvider = database.serviceProvider;

            //CREATION OF PRODUCTVIEWMODELS SO THEY CAN BE SAVED IN THE DATABASE
            var ProductToBeSaved1 = new ProductViewModel
            {
                Name = "new product",
                Description = "new product",
                Details = "new product",
                Stock = "3",
                Price = "35.1"
            };
            var ProductToBeSaved2 = new ProductViewModel
            {
                Name = "new product2",
                Description = "new product2 description",
                Details = "product2 details",
                Stock = "10",
                Price = "44.4"
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

            var product = FindID(MockProductService, ProductToBeSaved1);
            var product2 = FindID(MockProductService, ProductToBeSaved2);

            //ACT

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
            foreach (var order in NextOrders) //search the id of the new order
            {
                if (!ListIdPreviousOrders.Contains(order.Id)) { OrderId = order.Id; break; } //if the id is not in the older list then we found the id
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

        [Fact]
        public async void CartAddNewItems()
        {
            //ARANGE
            var database = new DataBaseSetup();
            var services = database.services;
            var serviceProvider = database.serviceProvider;

            //CREATION OF PRODUCTVIEWMODELS SO THEY CAN BE SAVED IN THE DATABASE
            var ProductToBeSaved1 = new ProductViewModel
            {
                Name = "new product",
                Description = "new product",
                Details = "new product",
                Stock = "3",
                Price = "35.1"
            };
            var ProductToBeSaved2 = new ProductViewModel
            {
                Name = "new product2",
                Description = "new product2 description",
                Details = "product2 details",
                Stock = "10",
                Price = "44.4"
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
            var product = FindID(MockProductService, ProductToBeSaved1);
            MockCartController.AddToCart(product.Id);

            MockProductController.Create(ProductToBeSaved2);
            var product2 = FindID(MockProductService, ProductToBeSaved2);
            MockCartController.AddToCart(product2.Id);


            var previousOrders = await MockOrderRepository.GetOrders();
            List<int> ListIdPreviousOrders = new List<int>();
            foreach (var order in previousOrders) { ListIdPreviousOrders.Add(order.Id); }

            MockOrderController.Index(new OrderViewModel()); //save our cart (from what i've seen, it also update the stock in the database without verifying if the articles are still in the database)

            //ASSERT
            var NextOrders = await MockOrderRepository.GetOrders();

            int OrderId = 0;
            foreach (var order in NextOrders) //search the id of the new order
            {
                if (!ListIdPreviousOrders.Contains(order.Id)) { OrderId = order.Id; break; } //if the id is not in the older list then we found the id
            }
            var OrderToTest = await MockOrderRepository.GetOrder(OrderId);

            Assert.Equal(2, OrderToTest.OrderLine.Count);
        }
    }
}
