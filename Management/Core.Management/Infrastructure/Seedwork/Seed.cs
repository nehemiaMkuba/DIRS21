using System;
using System.Threading.Tasks;

using IdGen;

using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Management.Interfaces;

namespace Core.Management.Infrastructure.Seedwork
{
    public class Seed : ISeed
    {
        private readonly IIdGenerator<long> _idGenerator;
        private readonly IProductRepository _productRepository;
        private readonly ISecurityRepository _securityRepository;
        public Seed(IIdGenerator<long> idGenerator, 
            ISecurityRepository securityRepository,
            IProductRepository productRepository)
        {
            _idGenerator = idGenerator;
            _productRepository = productRepository;
            _securityRepository = securityRepository;
        }
        public async Task SeedDefaults()
        {
            Client? rootClient = await _securityRepository.ValidateFindOneAsync(x => x.Role == Roles.Root, throwException: false).ConfigureAwait(false);
            // Create Defaults
            if (rootClient is null)
            {
                Client client = await _securityRepository.CreateClient("DIRS21", "nehemiah.mungau@gmx.ch", "Root user").ConfigureAwait(false);
                client = await _securityRepository.AssignClientRole(client.Id, Roles.Root).ConfigureAwait(false);
            }

            //Create Client Collection and seed indexes
            await _securityRepository.CreateCollection().ConfigureAwait(false);
            await _securityRepository.CreateIndexes().ConfigureAwait(false);

            //Create Products Collection and seed indexes
            await _productRepository.CreateCollection().ConfigureAwait(false);
            await _productRepository.CreateIndexes().ConfigureAwait(false);
        }
    }
}
