using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Core.Domain.Infrastructure.Services;

namespace Core.Domain.Entities.Documents
{
    public abstract class Document : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("createdAt")]
        public DateTime CreatedAt => new ObjectId(Id).CreationTime.ToInstanceDate();
    }
}