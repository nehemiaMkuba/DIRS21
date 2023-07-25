using System;
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
using Core.Management.Interfaces;

namespace Core.Management.Repositories
{
    public class ProductRepository : MongoRepository<Product>, IProductRepository
    {
        public ProductRepository(IMongoClient client,
           IDocumentSetting setting,
           IOptions<SecuritySetting> config,
           IConfiguration configuration) : base(client, setting.DatabaseName, setting.ProductsCollection)
        {

        }

        public async Task CreateCollection()
        {
            await CreateCollectionAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> CreateIndexes()
        {
            List<CreateIndexModel<Product>> indexKeysDefine = new List<CreateIndexModel<Product>>()
            {
                new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(indexKey => indexKey.CategoryName)),
                new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(indexKey => indexKey.Capacity)),
                new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(indexKey => indexKey.PricePerNight))
            };

            return await CreateIndexesAsync(indexKeysDefine).ConfigureAwait(false);
        }

        public async Task<Product> CreateProduct(ProductCategory categoryName, int capacity, decimal pricePerNight)
        {
            Product? product = await ValidateFindOneAsync(x => x.CategoryName == categoryName, throwException: false, inverseCheck: true).ConfigureAwait(false);

            product = new Product
            {
                CategoryName = categoryName,
                Capacity = capacity,
                PricePerNight = pricePerNight
            };

            await InsertOneAsync(product).ConfigureAwait(false);

            return product;
        }

        public async Task<bool> EditProduct(string id, ProductCategory? categoryName, int? capacity, decimal? pricePerNight)
        {
            Product product = await ValidateFindOneAsync(x => x.Id == id).ConfigureAwait(false);

            List<(Expression<Func<Product, object>> setExpression, object setFieldValue)> setExpression = new List<(Expression<Func<Product, object>> setExpression, object setFieldValue)>();

            if (categoryName.HasValue) setExpression.Add((x => x.CategoryName, categoryName));
            if (capacity.HasValue) setExpression.Add((x => x.Capacity, capacity));
            if (pricePerNight.HasValue) setExpression.Add((x => x.PricePerNight, pricePerNight));

            await UpdateOneAsync(x => x.Id, id, setExpression).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> Availability(ProductCategory categoryName, DateTime startDate, DateTime endDate)
        {
            Product product = await ValidateFindOneAsync(x => x.CategoryName == categoryName).ConfigureAwait(false);

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

            (IEnumerable<Product> tdocuments, long totalCount) = await FilterByPaginated(filter).ConfigureAwait(false);

            return (tdocuments, Convert.ToInt32(totalCount))!;
        }
    }
}
