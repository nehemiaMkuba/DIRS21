using Learning.Models;
using MediatR;

namespace Learning.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<Product>;
}
