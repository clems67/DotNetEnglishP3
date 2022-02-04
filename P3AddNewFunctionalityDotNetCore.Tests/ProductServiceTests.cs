using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using Xunit;
using Moq;
using System.Globalization;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        /// <summary>
        /// Take this test method as a template to write your test method.
        /// A test method must check if a definite method does its job:
        /// returns an expected value from a particular set of parameters
        /// </summary>
        /// 

        public class FakeLocalizer : IStringLocalizer<ProductService>
        {
            public LocalizedString this[string name]
            {
                get
                {
                    if (name == "MissingName")
                    {
                        return new LocalizedString("MissingName", "MissingName");
                    }
                    else if (name == "MissingPrice")
                    {
                        return new LocalizedString("MissingPrice", "MissingPrice");
                    }
                    else if (name == "PriceNotANumber")
                    {
                        return new LocalizedString("PriceNotANumber", "PriceNotANumber");
                    }
                    else if (name == "PriceNotGreaterThanZero")
                    {
                        return new LocalizedString("PriceNotGreaterThanZero", "PriceNotGreaterThanZero");
                    }
                    else if (name == "MissingQuantity")
                    {
                        return new LocalizedString("MissingQuantity", "MissingQuantity");
                    }
                    else if (name == "StockNotAnInteger")
                    {
                        return new LocalizedString("StockNotAnInteger", "StockNotAnInteger");
                    }
                    else //if (name == "StockNotGreaterThanZero")
                    {
                        return new LocalizedString("StockNotGreaterThanZero", "StockNotGreaterThanZero");
                    }
                }
            }

            public LocalizedString this[string name, params object[] arguments] => throw new System.NotImplementedException();

            public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            {
                throw new System.NotImplementedException();
            }

            public IStringLocalizer WithCulture(CultureInfo culture)
            {
                throw new System.NotImplementedException();
            }
        }

        [Fact]
        public void MissingNameTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            //ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = "color : orange";
            ProductToBeTested.Details = "Its diameter is 24cm.";
            ProductToBeTested.Stock = "1";
            ProductToBeTested.Price = "10";

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Contains("MissingName", productService1.CheckProductModelErrors(ProductToBeTested));
        }

        [Fact]
        public void MissingPriceTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = "color : orange";
            ProductToBeTested.Details = "Its diameter is 24cm.";
            ProductToBeTested.Stock = "1";
            ProductToBeTested.Price = ""; //price missing

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Contains("MissingPrice", productService1.CheckProductModelErrors(ProductToBeTested));
        }

        [Fact]
        public void PriceNotANumberTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = "color : orange";
            ProductToBeTested.Details = "Its diameter is 24cm.";
            ProductToBeTested.Stock = "1";
            ProductToBeTested.Price = "a"; //PriceNotANumber

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Contains("PriceNotANumber", productService1.CheckProductModelErrors(ProductToBeTested));
        }

        [Fact]
        public void PriceNotGreaterThanZeroTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = "color : orange";
            ProductToBeTested.Details = "Its diameter is 24cm.";
            ProductToBeTested.Stock = "1";
            ProductToBeTested.Price = "-5"; //PriceNotGreaterThanZero

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Contains("PriceNotGreaterThanZero", productService1.CheckProductModelErrors(ProductToBeTested));
        }

        [Fact]
        public void MissingQuantityTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = "color : orange";
            ProductToBeTested.Details = "Its diameter is 24cm.";
            ProductToBeTested.Stock = ""; //MissingQuantity
            ProductToBeTested.Price = "10";

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Contains("MissingQuantity", productService1.CheckProductModelErrors(ProductToBeTested));
        }

        [Fact]
        public void StockNotAnIntegerTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = "color : orange";
            ProductToBeTested.Details = "Its diameter is 24cm.";
            ProductToBeTested.Stock = "z"; //StockNotAnInteger
            ProductToBeTested.Price = "10";

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Contains("StockNotAnInteger", productService1.CheckProductModelErrors(ProductToBeTested));
        }

        [Fact]
        public void StockNotGreaterThanZeroTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = "color : orange";
            ProductToBeTested.Details = "Its diameter is 24cm.";
            ProductToBeTested.Stock = "-5"; //StockNotGreaterThanZero
            ProductToBeTested.Price = "10";

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Contains("StockNotGreaterThanZero", productService1.CheckProductModelErrors(ProductToBeTested));
        }

        [Fact]
        public void EverythingWorksTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();
            ProductToBeTested.Id = 1;
            ProductToBeTested.Name = "ball";
            ProductToBeTested.Description = ""; //the description can be empty
            ProductToBeTested.Details = ""; //the details can be empty
            ProductToBeTested.Stock = "5";
            ProductToBeTested.Price = "10";

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                new FakeLocalizer()
                );

            //ASSERT
            Assert.Empty(productService1.CheckProductModelErrors(ProductToBeTested));
        }

    }
}