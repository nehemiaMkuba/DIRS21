using Microsoft.AspNetCore.Mvc;

using FakeItEasy;
using AutoMapper;
using FluentAssertions;
using AutoMapper.Configuration;

using Core.Domain.Enums;
using Core.Domain.Entities;
using DIRS21.API.Controllers;
using Core.Management.Interfaces;
using DIRS21.API.Models.DTOs.Responses;
using DIRS21.API.Models.DTOs.Requests;

namespace DIRS21.API.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IProductRepository _productRepository;
        public ProductControllerTests()
        {
            _mapper = A.Fake<IMapper>();
            _configuration = A.Fake<IConfiguration>();
            _productRepository = A.Fake<IProductRepository>();
        }

        [Fact]
        public void ProductController_CreateProduct_ReturnsCreated()
        {
            #region Arrange

            Product productEntity = A.Fake<Product>();
            ProductDto productDto = A.Fake<ProductDto>();
            ProductRequest request = A.Fake<ProductRequest>();

            A.CallTo(() => _mapper.Map<ProductDto>(productEntity)).Returns(productDto);
            A.CallTo(() => _productRepository.CreateProduct(request.CategoryName, request.Capacity, request.PricePerNight)).Returns(productEntity);

            ProductsController controller = new ProductsController(_mapper, _productRepository);

            #endregion

            #region Act

            var result = controller.CreateProduct(request).Result;

            #endregion

            #region Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<CreatedResult>();
            #endregion
        }

        [Fact]
        public void ProductController_EditProduct_ReturnsOk()
        {
            #region Arrange

            EditProductRequest request = A.Fake<EditProductRequest>();
            bool updateResult = true;

            A.CallTo(() => _productRepository.EditProduct(request.Id, request.CategoryName, request.Capacity, request.PricePerNight)).Returns(updateResult);

            ProductsController controller = new ProductsController(_mapper, _productRepository);

            #endregion

            #region Act

            var result = controller.EditProduct(request).Result;

            #endregion

            #region Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();


            #endregion
        }

        [Fact]
        public void ProductController_GetProduct_ReturnsOk()
        {
            #region Arrange

            string request = "64b8f701c3275fb645a5ca5e";
            Product productEntity = A.Fake<Product>();
            ProductDto productDto = A.Fake<ProductDto>();

            A.CallTo(() => _productRepository.FindByIdAsync(request)).Returns(productEntity);
            A.CallTo(() => _mapper.Map<ProductDto>(productEntity)).Returns(productDto);

            ProductsController controller = new ProductsController(_mapper, _productRepository);

            #endregion

            #region Act

            var result = controller.GetProduct(request).Result;

            #endregion

            #region Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();


            #endregion
        }

        [Fact]
        public void ProductController_DeleteProduct_ReturnsOk()
        {
            #region Arrange

            string id = "1";
            bool updateResult = true;

            A.CallTo(() => _productRepository.DeleteByIdAsync(id)).Returns(updateResult);

            ProductsController controller = new ProductsController(_mapper, _productRepository);

            #endregion

            #region Act

            IActionResult? result = controller.DeleteProduct(id).Result;
            
            #endregion

            #region Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();


            #endregion
        }
        [Fact]
        public void ProductController_GetProducts_ReturnsOk()
        {
            #region Arrange

            ProductCategory? categoryName = ProductCategory.SingleRoom;

            int? capacity = 1; decimal? pricePerNight = 1000; string startingAfterProductId = "1"; string endingBeforeProductId = "20";

            IEnumerable<Product> products = A.Fake<IEnumerable<Product>>();
            IEnumerable<ProductDto> productDtos = A.Fake<IEnumerable<ProductDto>>();

            A.CallTo(() => _productRepository.GetProductList(categoryName, capacity, pricePerNight, startingAfterProductId, endingBeforeProductId)).Returns((products,10));
            A.CallTo(() => _mapper.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            ProductsController controller = new ProductsController(_mapper, _productRepository);

            #endregion

            #region Act

            var result = controller.GetProducts(categoryName, capacity, pricePerNight, startingAfterProductId, endingBeforeProductId).Result;

            #endregion

            #region Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();


            #endregion
        }
    }
}
