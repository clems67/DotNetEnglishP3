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

            OrderController orderController = serviceProvider.GetService<OrderController>();
            ProductController productController = serviceProvider.GetService<ProductController>();
            CartController cartController = serviceProvider.GetService<CartController>();
            ProductService productService = serviceProvider.GetService<ProductService>();
            OrderRepository orderRepository = serviceProvider.GetRequiredService<OrderRepository>();
            productController.Create(ProductToBeSaved1);
            productController.Create(ProductToBeSaved2);

            var product = FindID(productService, ProductToBeSaved1);
            var product2 = FindID(productService, ProductToBeSaved2);

            //ACT

            cartController.AddToCart(product.Id);
            cartController.AddToCart(product2.Id);

            productController.DeleteProduct(product.Id);

            var previousOrders = await orderRepository.GetOrders();
            List<int> ListIdPreviousOrders = new List<int>();
            foreach (var order in previousOrders) { ListIdPreviousOrders.Add(order.Id); }

            orderController.Index(new OrderViewModel());

            //ASSERT
            var NextOrders = await orderRepository.GetOrders();

            int OrderId = 0;
            foreach (var order in NextOrders) //search the id of the new order
            {
                if (!ListIdPreviousOrders.Contains(order.Id)) { OrderId = order.Id; break; } //if the id is not in the older list then we found the id
            }
            var OrderToTest = await orderRepository.GetOrder(OrderId);

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

            OrderController orderController = serviceProvider.GetService<OrderController>();
            ProductController productController = serviceProvider.GetService<ProductController>();
            CartController CartController = serviceProvider.GetService<CartController>();
            ProductService productService = serviceProvider.GetService<ProductService>();
            OrderRepository orderRepository = serviceProvider.GetRequiredService<OrderRepository>();

            //ACT

            productController.Create(ProductToBeSaved1);
            var product = FindID(productService, ProductToBeSaved1);
            CartController.AddToCart(product.Id);

            productController.Create(ProductToBeSaved2);
            var product2 = FindID(productService, ProductToBeSaved2);
            CartController.AddToCart(product2.Id);


            var previousOrders = await orderRepository.GetOrders();
            List<int> ListIdPreviousOrders = new List<int>();
            foreach (var order in previousOrders) { ListIdPreviousOrders.Add(order.Id); }

            orderController.Index(new OrderViewModel());

            //ASSERT
            var NextOrders = await orderRepository.GetOrders();

            int OrderId = 0;
            foreach (var order in NextOrders) //search the id of the new order
            {
                if (!ListIdPreviousOrders.Contains(order.Id)) { OrderId = order.Id; break; } //if the id is not in the older list then we found the id
            }
            var OrderToTest = await orderRepository.GetOrder(OrderId);

            Assert.Equal(2, OrderToTest.OrderLine.Count);
        }
    }
}
