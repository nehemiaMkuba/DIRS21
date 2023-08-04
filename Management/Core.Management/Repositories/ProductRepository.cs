using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using Core.Domain.Enums;
using Core.Domain.Entities;
using Core.Management.Common;
using Core.Domain.Exceptions;
using Core.Management.Interfaces;

namespace Core.Management.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoRepository<Product> _productDataServiceFactory;
        public ProductRepository(IMongoRepository<Product> productDataServiceFactory)
        {
            _productDataServiceFactory = productDataServiceFactory;
        }

        public async Task CreateCollection()
        {
            await _productDataServiceFactory.CreateCollectionAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> CreateIndexes()
        {
            List<CreateIndexModel<Product>> indexKeysDefine = new List<CreateIndexModel<Product>>()
            {
                new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(indexKey => indexKey.CategoryName)),
                new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(indexKey => indexKey.Capacity)),
                new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(indexKey => indexKey.PricePerNight))
            };

            return await _productDataServiceFactory.CreateIndexesAsync(indexKeysDefine).ConfigureAwait(false);
        }

        public async Task<Product> CreateProduct(ProductCategory categoryName, int capacity, decimal pricePerNight)
        {
            Product? product = await _productDataServiceFactory.ValidateFindOneAsync(x => x.CategoryName == categoryName, throwException: false, inverseCheck: true).ConfigureAwait(false);

            if (capacity < 1) throw new GenericException("capacity value cannot be less than 1", "DIRS21009", HttpStatusCode.PreconditionFailed);
            if (pricePerNight < 1) throw new GenericException("pricePerNight value cannot be less than 1", "DIRS21010", HttpStatusCode.PreconditionFailed);

            product = new Product
            {
                CategoryName = categoryName,
                Capacity = capacity,
                PricePerNight = pricePerNight
            };

            await _productDataServiceFactory.InsertOneAsync(product).ConfigureAwait(false);

            return product;
        }

        public async Task<bool> EditProduct(string id, ProductCategory? categoryName, int? capacity, decimal? pricePerNight)
        {
            Product product = await _productDataServiceFactory.ValidateFindOneAsync(x => x.Id == id).ConfigureAwait(false);

            if (capacity.HasValue && capacity < 1) throw new GenericException("capacity value cannot be less than 1", "DIRS21009", HttpStatusCode.PreconditionFailed);
            if (pricePerNight.HasValue && pricePerNight < 1) throw new GenericException("pricePerNight value cannot be less than 1", "DIRS21010", HttpStatusCode.PreconditionFailed);


            List<(Expression<Func<Product, object>> setExpression, object setFieldValue)> setExpression = new List<(Expression<Func<Product, object>> setExpression, object setFieldValue)>();

            if (categoryName.HasValue) setExpression.Add((x => x.CategoryName, categoryName));
            if (capacity.HasValue) setExpression.Add((x => x.Capacity, capacity));
            if (pricePerNight.HasValue) setExpression.Add((x => x.PricePerNight, pricePerNight));

            await _productDataServiceFactory.UpdateOneAsync(x => x.Id, id, setExpression).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> Availability(ProductCategory categoryName, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate) throw new GenericException("End date must be greater than startDate", "DIRS21009", HttpStatusCode.PreconditionFailed);

            Product product = await _productDataServiceFactory.ValidateFindOneAsync(x => x.CategoryName == categoryName).ConfigureAwait(false);

            return !(product.BookedDates.Any(x => x.StartAt >= startDate && x.EndAt <= endDate));
        }

        public async Task<(IEnumerable<Product> products, int totalCount)> GetProductList(ProductCategory? categoryName, int? capacity, decimal? pricePerNight, string startingAfterProductId, string endingBeforeProductId)
        {
            FilterDefinitionBuilder<Product> builder = Builders<Product>.Filter;
            FilterDefinition<Product> filter = builder.Empty;

            if (categoryName.HasValue) filter &= builder.Eq(x => x.CategoryName, categoryName);

            if (capacity.HasValue) filter &= builder.Eq(x => x.Capacity, capacity);

            if (pricePerNight > 0) filter &= builder.Eq(x => x.PricePerNight, pricePerNight);

            if (!string.IsNullOrEmpty(startingAfterProductId)) filter &= builder.Gt(x => x.Id, startingAfterProductId);

            if (!string.IsNullOrEmpty(endingBeforeProductId)) filter &= builder.Lt(x => x.Id, endingBeforeProductId);

            (IEnumerable<Product> tdocuments, long totalCount) = await _productDataServiceFactory.FilterByPaginated(filter).ConfigureAwait(false);

            return (tdocuments, Convert.ToInt32(totalCount))!;
        }

        public async Task<Product> ValidateFindOneAsync(string id)
        {
            return await _productDataServiceFactory.ValidateFindOneAsync(x => x.Id == id).ConfigureAwait(false);
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            return await _productDataServiceFactory.DeleteByIdAsync(id).ConfigureAwait(false);
        }
    }
}
