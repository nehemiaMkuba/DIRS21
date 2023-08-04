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

    private readonly IMongoRepository<Product> _productDataServiceFactory;
    public ProductRepositoryTests()
    {
        _productDataServiceFactory = A.Fake<IMongoRepository<Product>>();
    }

    [Theory]
    [ClassData(typeof(ProductTestData))]
    public async Task ProductRepository_CreateProduct_ThrowsException(ProductCategory categoryName, int capacity, int pricePerNight, Object response)
    {
        #region Arrange

        ProductRepository productRepository = new ProductRepository(_productDataServiceFactory);

        Product request = new() { CategoryName = categoryName, Capacity = capacity, PricePerNight = Convert.ToDecimal(pricePerNight) };

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
    public async Task ProductRepository_EditProduct_ThrowsException(string id, ProductCategory? categoryName, int? capacity, int? pricePerNight, Object response)
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

    [Theory]
    [ClassData(typeof(CheckAvailabilityTestData))]
    public async Task ProductRepository_Availability_ThrowsException(ProductCategory categoryName, DateTime startDate, DateTime endDate, Object response)
    {

        #region Arrange
        ProductRepository productRepository = new ProductRepository(_productDataServiceFactory);

        A.CallTo(() => _productDataServiceFactory.ValidateFindOneAsync(x => x.CategoryName == categoryName));

        #endregion

        #region Act

        Func<Task> act = async () => await productRepository.Availability(categoryName, startDate, endDate);

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
        yield return new object[] { ProductCategory.DoubleRoom, 1, 0, "pricePerNight value cannot be less than 1" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class EditProductTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1, ProductCategory.DoubleRoom, 0, 1, "capacity value cannot be less than 1" };
        yield return new object[] { 2, ProductCategory.DoubleRoom, 1, 0, "pricePerNight value cannot be less than 1" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class CheckAvailabilityTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { ProductCategory.DoubleRoom,DateTime.Today, DateTime.Today.AddDays(-1), "End date must be greater than startDate" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}