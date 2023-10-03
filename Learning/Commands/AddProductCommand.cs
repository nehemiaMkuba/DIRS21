using Learning.Models;
using MediatR;

namespace Learning.Commands
{
    public record AddProductCommand(Product Product) : IRequest<Product>;
}
