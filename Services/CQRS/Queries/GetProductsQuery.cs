using CQRS.Models;
using MediatR;

namespace CQRS.Queries
{
    public record GetProductsQuery() : IRequest<IEnumerable<Product>>;
}
