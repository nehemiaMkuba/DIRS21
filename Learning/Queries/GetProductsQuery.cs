using Learning.Models;
using MediatR;

namespace Learning.Queries
{
    public record GetProductsQuery() : IRequest<IEnumerable<Product>>;
}
