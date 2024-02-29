using CQRS.Models;
using MediatR;

namespace CQRS.Notifications
{
    public record ProductAddedNotification(Product Product) : INotification;
}
