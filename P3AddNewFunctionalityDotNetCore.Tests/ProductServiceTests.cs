using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

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
                    switch (name)
                    {
                        case "MissingName":
                            return new LocalizedString("MissingName", "MissingName");
                        case "MissingPrice":
                            return new LocalizedString("MissingPrice", "MissingPrice");
                        case "PriceNotANumber":
                            return new LocalizedString("PriceNotANumber", "PriceNotANumber");
                        case "PriceNotGreaterThanZero":
                            return new LocalizedString("PriceNotGreaterThanZero", "PriceNotGreaterThanZero");
                        case "MissingQuantity":
                            return new LocalizedString("MissingQuantity", "MissingQuantity");
                        case "StockNotAnInteger":
                            return new LocalizedString("StockNotAnInteger", "StockNotAnInteger");
                        case "StockNotGreaterThanZero":
                            return new LocalizedString("StockNotGreaterThanZero", "StockNotGreaterThanZero");
                        default:
                            return new LocalizedString("", "");
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

        public IStringLocalizer<ProductService> MockLocaliser()
        {
            var MockL = new Mock<IStringLocalizer<ProductService>>();
            MockL.Setup(_ => _["MissingName"]).Returns(new LocalizedString("MissingName", "MissingName"));
            MockL.Setup(_ => _["MissingPrice"]).Returns(new LocalizedString("MissingPrice", "MissingPrice"));
            MockL.Setup(_ => _["PriceNotANumber"]).Returns(new LocalizedString("PriceNotANumber", "PriceNotANumber"));
            MockL.Setup(_ => _["PriceNotGreaterThanZero"]).Returns(new LocalizedString("PriceNotGreaterThanZero", "PriceNotGreaterThanZero"));
            MockL.Setup(_ => _["MissingQuantity"]).Returns(new LocalizedString("MissingQuantity", "MissingQuantity"));
            MockL.Setup(_ => _["StockNotAnInteger"]).Returns(new LocalizedString("StockNotAnInteger", "StockNotAnInteger"));
            MockL.Setup(_ => _["StockNotGreaterThanZero"]).Returns(new LocalizedString("StockNotGreaterThanZero", "StockNotGreaterThanZero"));

            return MockL.Object;
        }

        public ProductViewModel ProductVM()
        {
            ProductViewModel ProductToBeTested = new ProductViewModel
            {
                Id = 1,
                Name = "ball",
                Description = "color : orange",
                Details = "Its diameter is 24cm.",
                Stock = "1",
                Price = "10.03"
            };

            return ProductToBeTested;
        }

        [Fact]
        public void MissingNameTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();
            ProductToBeTested.Name = "";
            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                //new FakeLocalizer()
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);


            //ASSERT
            Assert.Contains("MissingName", ErrorList); //ok
            Assert.Single(ErrorList);
        }

        [Fact]
        public void MissingPriceTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();
            ProductToBeTested.Price = " ";
            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                //new FakeLocalizer()
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);


            //ASSERT
            Assert.Contains("MissingPrice", ErrorList);
             Assert.Single(ErrorList);
        }

        [Fact]
        public void PriceNotANumberTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();
            ProductToBeTested.Price = "a"; //PriceNotANumber
            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);

            //ASSERT
            Assert.Contains("PriceNotANumber", ErrorList); // +PriceNotGreaterThanZero
            Assert.Single(ErrorList);
        }

        [Fact]
        public void PriceNotGreaterThanZeroTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();
            ProductToBeTested.Price = "-5"; //PriceNotGreaterThanZero

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);

            //ASSERT
            Assert.Contains("PriceNotANumber", ErrorList); // +PriceNotANumber
            Assert.Single(ErrorList);
        }

        [Fact]
        public void MissingQuantityTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();
            ProductToBeTested.Stock = " "; //MissingQuantity

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);

            //ASSERT
            Assert.Contains("MissingQuantity", ErrorList);
            Assert.Single(ErrorList);
        }

        [Fact]
        public void StockNotAnIntegerTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();
            ProductToBeTested.Stock = "z"; //StockNotAnInteger

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);

            //ASSERT
            Assert.Contains("StockNotAnInteger", ErrorList); // 2 times : "StockNotAnInteger"
            Assert.Single(ErrorList);
        }

        [Fact]
        public void StockNotGreaterThanZeroTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();
            ProductToBeTested.Stock = "-5"; //StockNotGreaterThanZero

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);

            //ASSERT
            Assert.Contains("StockNotAnInteger", ErrorList); // 2 times : "StockNotAnInteger"
            Assert.Single(ErrorList);
        }

        [Fact]
        public void EverythingWorksTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = ProductVM();

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);

            //ASSERT
            Assert.Empty(ErrorList);
        }

        [Fact]
        public void NothingWorksTest()
        {
            //ARRANGE
            ProductViewModel ProductToBeTested = new ProductViewModel();

            var MockCart = new Mock<ICart>();
            var MockProductRepository = new Mock<IProductRepository>();
            var MockOrderRepository = new Mock<IOrderRepository>();

            //ACT
            ProductService productService1 = new ProductService(
                MockCart.Object,
                MockProductRepository.Object,
                MockOrderRepository.Object,
                MockLocaliser()
                );
            List<string> ErrorList = productService1.CheckProductModelErrors(ProductToBeTested);

            //ASSERT
            Assert.Equal(3, ErrorList.Count);
        }
    }
}