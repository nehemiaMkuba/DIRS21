﻿using System.Threading.Tasks;
using System.Collections.Generic;

using Core.Domain.Enums;
using Core.Domain.Entities;
using System;

namespace Core.Management.Interfaces
{
    public interface IProductRepository: IMongoRepository<Product>
    {
        Task CreateCollection();
        Task<IEnumerable<string>> CreateIndexes();
        Task<Product> CreateProduct(ProductCategory categoryName, int capacity, decimal pricePerNight);
        Task<bool> EditProduct(string id, ProductCategory? categoryName, int? capacity, decimal? pricePerNight);
        Task<bool> Availability(ProductCategory categoryName, DateTime startDate, DateTime endDate);
        Task<(IEnumerable<Product> products, int totalCount)> GetProductList(ProductCategory? categoryName, int? capacity, decimal? pricePerNight, string startingAfterProductId, string endingBeforeProductId);
    }
}
