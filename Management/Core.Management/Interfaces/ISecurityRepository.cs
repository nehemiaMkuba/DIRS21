using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Core.Domain.Enums;
using Core.Domain.Entities;
using System.Linq.Expressions;

namespace Core.Management.Interfaces
{
    public interface ISecurityRepository
    {
        Task CreateCollection();
        Task<IEnumerable<string>> CreateIndexes();
        Task<Client> CreateClient(string name, string contactEmail, string description);
        Task<Client> ValidateFindOneAsync(Expression<Func<Client, bool>> filterExpression, bool throwException = false);
        Task<Client> AssignClientRole(string clientId, Roles role);
        (string token, long expires) CreateAccessToken(Client client);
        Task<(string token, long expires)> ExtendAccessTokenLifetime(string accessToken, string appSecret);
        Task<Client> AuthenticateClient(string apiKey, string appSecret);
        Task<Client> GetClientById(string clientId);
        Task<List<Client>> GetClients();
        bool ValidateServerKey(string apiKey);
    }
}