using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Core.Domain.Enums;
using Core.Domain.Entities;

namespace Core.Management.Interfaces
{
    public interface IProductRepository
    {
        Task CreateCollection();
        Task<IEnumerable<string>> CreateIndexes();
        Task<Product> CreateProduct(ProductCategory categoryName, int capacity, decimal pricePerNight);
        Task<bool> EditProduct(string id, ProductCategory? categoryName, int? capacity, decimal? pricePerNight);
        Task<Product> ValidateFindOneAsync(string id);
        Task<bool> DeleteByIdAsync(string id);
        Task<bool> Availability(ProductCategory categoryName, DateTime startDate, DateTime endDate);
        Task<(IEnumerable<Product> products, int totalCount)> GetProductList(ProductCategory? categoryName, int? capacity, decimal? pricePerNight, string startingAfterProductId, string endingBeforeProductId);
    }
}
