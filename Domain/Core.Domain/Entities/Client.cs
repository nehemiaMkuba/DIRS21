using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Core.Domain.Common;
using Core.Domain.Enums;
using Core.Domain.Entities.Documents;

namespace Core.Domain.Entities
{
    public class Client : Document
    {
        [BsonElement("category")]
        public string Name { get; set; }

        [BsonElement("secret")]
        public string Secret { get; set; }

        [BsonElement("role"), BsonRepresentation(BsonType.String)]
        public Roles Role { get; set; }

        [BsonElement("acessTokenLifetimeInMins")]
        public int AccessTokenLifetimeInMins { get; set; } = 60;

        [BsonElement("authorizationCodeLifetimeInMins")]
        public int AuthorizationCodeLifetimeInMins { get; set; } = 60;

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = false;

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("contactEmail")]
        public string ContactEmail { get; set; }
    }
}