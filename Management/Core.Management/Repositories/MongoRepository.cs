using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Driver;

using Core.Domain.Exceptions;
using Core.Management.Interfaces;
using Core.Domain.Entities.Documents;

namespace Core.Management.Repositories
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _database;
        protected readonly IMongoCollection<TDocument> _collection;
        public MongoRepository(IMongoClient client, string databaseName, string collectionName)
        {
            _collectionName = collectionName;
            _database = client.GetDatabase(databaseName);
            _collection = _database.GetCollection<TDocument>(collectionName);
        }

        public async Task CreateCollectionAsync()
        {
            IAsyncCursor<BsonDocument> collections = await _database.ListCollectionsAsync(new ListCollectionsOptions { Filter = new BsonDocument("name", _collectionName) });
           
            if (!await collections.AnyAsync().ConfigureAwait(false))
            {
                await _database.CreateCollectionAsync(_collectionName, options: new CreateCollectionOptions() { Collation = new Collation("en_US", strength: CollationStrength.Secondary) }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<string>> CreateIndexesAsync(IEnumerable<CreateIndexModel<TDocument>> indexModels)
        {
            return await _collection.Indexes.CreateManyAsync(indexModels).ConfigureAwait(false);
        }

        public async Task InsertOneAsync(TDocument document)
        {
            await _collection.InsertOneAsync(document).ConfigureAwait(false);
        }

        public async Task<TDocument> FindByIdAsync(string id)
        {
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);

            return await _collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<TDocument> ValidateFindOneAsync(Expression<Func<TDocument, bool>> filterExpression, bool inverseCheck = false, bool throwException = true)
        {
            TDocument entity = await _collection.Find(filterExpression).FirstOrDefaultAsync().ConfigureAwait(false);

            Type type = typeof(TDocument);

            if (entity != null && inverseCheck)
                throw new GenericException($" {type.Name} is already registered", "DIRS21002", HttpStatusCode.Conflict);

            if (entity is null && throwException)
                throw new GenericException($" {type.Name} could not be found", "DIRS21001", HttpStatusCode.NotFound);

            return entity;
        }

        public async Task<long> CountDocuments(Expression<Func<TDocument, bool>> filterExpression)
        {
            return await _collection.CountDocumentsAsync(filterExpression).ConfigureAwait(false);
        }

        public async Task UpdateOneAsync<TField>(Expression<Func<TDocument, TField>> filterExpression, TField fieldValue, List<(Expression<Func<TDocument, TField>> setExpression, TField setFieldValue)> setExpressions)
        {
            FilterDefinition<TDocument> filterDefinition = Builders<TDocument>.Filter.Eq(filterExpression, fieldValue);

            UpdateDefinitionBuilder<TDocument> updateDefinitionBuilder = Builders<TDocument>.Update;
            List<UpdateDefinition<TDocument>> updateDefinitions = new List<UpdateDefinition<TDocument>>();

            foreach ((Expression<Func<TDocument, TField>> setExpression, TField setFieldValue) in setExpressions)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set(setExpression, setFieldValue));
            }

            await _collection.UpdateOneAsync(filterDefinition, updateDefinitionBuilder.Combine(updateDefinitions)).ConfigureAwait(false);
        }

        public async Task DeleteByIdAsync(string id)
        {
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            
            await _collection.DeleteOneAsync(filter).ConfigureAwait(false); ;
        }

        public async Task<List<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
        {
            return await _collection.Find(filterExpression).ToListAsync().ConfigureAwait(false); ;
        }

        public async Task<(IEnumerable<TDocument> tdocuments, int newStartIndex, int newPageSize, long totalCount)> FilterByPaginated(Expression<Func<TDocument, bool>> filterExpression, int offset, int pageSize)
        {
            Task<long> totalCountTask = _collection.CountDocumentsAsync(filterExpression);
            Task<List<TDocument>> collectionsTask = _collection.Find(filterExpression).Skip(offset).Limit(pageSize).ToListAsync();

            await Task.WhenAll(totalCountTask, collectionsTask).ConfigureAwait(false);

            return (collectionsTask.Result,
            offset + pageSize, pageSize, totalCountTask.Result)!;
        }

        public async Task<(IEnumerable<TDocument> tdocuments, long totalCount)> FilterByPaginated(FilterDefinition<TDocument> filterDefinition)
        {
            Task<long> totalCountTask = _collection.CountDocumentsAsync(filterDefinition);
            Task<List<TDocument>> collectionsTask = _collection.Find(filterDefinition).SortByDescending(x => x.Id).ToListAsync();

            await Task.WhenAll(totalCountTask, collectionsTask).ConfigureAwait(false);

            return (collectionsTask.Result,
              totalCountTask.Result)!;
        }
    }
}
