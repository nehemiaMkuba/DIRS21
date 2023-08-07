using Core.Domain.Enums;

namespace DIRS21.API.Models.DTOs.Responses
{
    public class ProductDto
    {
        public string Id { get; set; }
        public ProductCategory CategoryName { get; set; }

        public int Capacity { get; set; }

        public decimal PricePerNight { get; set; }
    }
}
