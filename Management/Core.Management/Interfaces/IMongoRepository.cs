using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using MongoDB.Driver;

using Core.Domain.Enums;
using System.Linq.Expressions;
using Core.Domain.Entities.Documents;

namespace Core.Management.Interfaces {
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        Task CreateCollectionAsync();

        Task<IEnumerable<string>> CreateIndexesAsync(IEnumerable<CreateIndexModel<TDocument>> indexModels);

        Task InsertOneAsync(TDocument document);

        Task<TDocument> FindByIdAsync(string id);

        Task<TDocument> ValidateFindOneAsync(Expression<Func<TDocument, bool>> filterExpression, bool inverseCheck = false, bool throwException = true);

        Task<long> CountDocuments(Expression<Func<TDocument, bool>> filterExpression);

        IQueryable<TDocument> AsQueryable();

        Task UpdateOneAsync<TField>(Expression<Func<TDocument, TField>> filterExpression, TField fieldValue, List<(Expression<Func<TDocument, TField>> setExpression, TField setFieldValue)> setExpressions);

        Task<bool> DeleteByIdAsync(string id);

        Task<List<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression);
        Task<(IEnumerable<TDocument> tdocuments, int newStartIndex, int newPageSize, long totalCount)> FilterByPaginated(Expression<Func<TDocument, bool>> filterExpression, int offset, int pageSize);
        Task<(IEnumerable<TDocument> tdocuments, long totalCount)> FilterByPaginated(FilterDefinition<TDocument> filterDefinition);

    }
}

