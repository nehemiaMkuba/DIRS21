using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IdGen;
using MongoDB.Driver;

using Core.Management.Common;
using Core.Management.Interfaces;
using Core.Management.Repositories;
using Core.Management.Infrastructure.Seedwork;
using Core.Management.Infrastructure.IntegrationEvents.EventBus;

namespace Core.Management
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<ISeed, Seed>();
            services.AddTransient<IIdGenerator<long>>(_ => new IdGenerator(0, new IdGeneratorOptions(idStructure: new IdStructure(45, 2, 16), timeSource: new DefaultTimeSource(new DateTime(2021, 4, 23, 11, 0, 0, DateTimeKind.Utc)))));
            
            services.AddScoped<ISecurityRepository, SecurityRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            services.AddSingleton<IDocumentSetting>(provider => provider.GetRequiredService<IOptions<DocumentSetting>>().Value);
            services.AddSingleton<ISecuritySetting>(provider => provider.GetRequiredService<IOptions<SecuritySetting>>().Value);

            services.AddScoped<IQueueService, QueueService>();

            return services;
        }
    }
}