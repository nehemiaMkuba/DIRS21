using Core.Domain.Enums;

namespace Core.Domain.Entities
{
    public interface IProduct
    {
        ProductCategory CategoryName { get; }
        int Capacity { get; }
        decimal PricePerNight { get; }

    }
}
