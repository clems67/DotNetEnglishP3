using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductControllerTest : DataBaseTest
    {
        public readonly IProductService productService;
        public readonly ProductService productService2;
        public readonly ILanguageService languageService;
        public readonly IProductRepository productRepository;
        public readonly ICart cart;
        public readonly IOrderRepository orderRepository;
        public readonly StringLocalizer<ProductService> localizer;
        public ProductViewModel product;
        public readonly ProductController productController;
        public readonly P3Referential p3Referential;
        public ProductControllerTest()
        {
            p3Referential = new P3Referential(builder);

            productRepository = new ProductRepository(p3Referential);
            productService = new ProductService(cart, new ProductRepository(p3Referential), orderRepository, localizer);

            productController = new ProductController(productService, languageService);
            product = new ProductViewModel();
        }
    }
}
