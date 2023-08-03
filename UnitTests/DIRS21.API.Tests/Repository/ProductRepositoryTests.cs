using System.Net;
using System.Collections;
using System.Linq.Expressions;

using AutoMapper;


using FakeItEasy;
using MongoDB.Driver;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

using Core.Domain.Enums;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Management.Common;
using Core.Management.Interfaces;
using Core.Management.Repositories;
using Core.Domain.Infrastructure.Services;


namespace DIRS21.API.Tests.Repository;

public class ProductRepositoryTests
{
    private readonly IMapper _mapper;
    private readonly IMongoClient _client;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeService _dateTimeService;
    private readonly IDocumentSetting _documentSetting;
    private readonly IOptions<SecuritySetting> _securitySetting;
    private readonly IMongoRepository<Product> _productDataServiceFactory;
    public ProductRepositoryTests()
    {
        _mapper = A.Fake<IMapper>();
        _client = A.Fake<IMongoClient>();
        _configuration = A.Fake<IConfiguration>();
        _dateTimeService = A.Fake<IDateTimeService>();
        _documentSetting = A.Fake<IDocumentSetting>();
        _securitySetting = A.Fake<IOptions<SecuritySetting>>();
        _productDataServiceFactory = A.Fake<IMongoRepository<Product>>();
    }

    [Theory]
    [ClassData(typeof(ProductTestData))]
    public async Task ProductRepository_CreateProduct_ThrowsException(ProductCategory categoryName, int capacity, decimal pricePerNight, Object response)
    {
        #region Arrange

        ProductRepository productRepository = new ProductRepository(_productDataServiceFactory);

        Product request = new() { CategoryName = categoryName, Capacity = capacity, PricePerNight = pricePerNight };

        A.CallTo(() => _productDataServiceFactory.ValidateFindOneAsync(x => x.CategoryName == categoryName));

        A.CallTo(() => _productDataServiceFactory.InsertOneAsync(request));

        #endregion

        #region Act

        Func<Task> act = async () => await productRepository.CreateProduct(categoryName, capacity, pricePerNight);

        #endregion

        #region Assert

        await act.Should().ThrowAsync<GenericException>()
                .WithMessage(response.ToString());

        #endregion
    }

    [Theory]
    [ClassData(typeof(EditProductTestData))]
    public async Task ProductRepository_EditProduct_ReturnsOk(string id, ProductCategory? categoryName, int? capacity, decimal? pricePerNight, Object response)
    {

        #region Arrange

        List<(Expression<Func<Product, object>> setExpression, object setFieldValue)> setExpression = new List<(Expression<Func<Product, object>> setExpression, object setFieldValue)>();

        ProductRepository productRepository = new ProductRepository(_productDataServiceFactory);

        A.CallTo(() => _productDataServiceFactory.ValidateFindOneAsync(x => x.Id == id));

        A.CallTo(() => _productDataServiceFactory.UpdateOneAsync(x => x.Id, id, setExpression));

        #endregion

        #region Act

        Func<Task> act = async () => await productRepository.EditProduct(id, categoryName, capacity, pricePerNight);

        #endregion

        #region Assert

        await act.Should().ThrowAsync<GenericException>()
                .WithMessage(response.ToString());

        #endregion

    }

 

}

public class ProductTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { ProductCategory.DoubleRoom, 0, 1, "capacity value cannot be less than 1" };
        yield return new object[] { ProductCategory.DoubleRoom, 1, 0, "pricePerNight value cannot be less than 1", "DIRS21010" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class EditProductTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1, ProductCategory.DoubleRoom, 0, 1, "capacity value cannot be less than 1" };
        yield return new object[] { 2, ProductCategory.DoubleRoom, 1, 0, "pricePerNight value cannot be less than 1"};
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}