using Learning.Models;
using MediatR;

namespace Learning.Notifications
{
    public record ProductAddedNotification(Product Product) : INotification;
}
