using Learning.Models;
using Learning.Notifications;
using MediatR;

namespace Learning.Handlers
{
    public class CacheInvalidationHandler : INotificationHandler<ProductAddedNotification>
    {
        private readonly FakeDataStore _fakeDataStore;
        public CacheInvalidationHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;
        public async Task Handle(ProductAddedNotification notification, CancellationToken cancellationToken)
        {
            await _fakeDataStore.EventOccured(notification.Product, "Cache Invalidated");
            await Task.CompletedTask;
        }
    }
}
