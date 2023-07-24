using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using Core.Domain.Enums;
using Core.Domain.Infrastructure.Services;

namespace Core.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddSingleton<IMongoClient>(_ => new MongoClient(MongoClientSettings.FromConnectionString(configuration.GetConnectionString("DocumentConnection"))));

            return services;
        }
    }
}
