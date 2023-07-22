using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Core.Domain.Enums;

namespace Core.Domain.Entities
{
    public class Product : BaseProduct, IProduct
    {
        [BsonElement("categoryName"), BsonRepresentation(BsonType.String)]
        public ProductCategory CategoryName { get; set; }

        [BsonElement("capacity")]
        public int Capacity { get; set; }

        [BsonElement("pricePerNight")]
        public decimal PricePerNight { get; set; }

        [BsonElement("bookedDates")]
        public BookedDates[] BookedDates { get; set; }
    }

    public class BookedDates
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
