using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using Core.Domain.Enums;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Management.Common;
using Core.Management.Extensions;
using Core.Management.Interfaces;

namespace Core.Management.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ISecuritySetting _securitySetting;
        private readonly IMongoRepository<Client>  _clientDataServiceFactory;


        public SecurityRepository(
           IMongoRepository<Client> clientDataServiceFactory,
            IOptions<SecuritySetting> config,
            IConfiguration configuration) 
        {
            _configuration = configuration;
            _securitySetting = config.Value;
            _clientDataServiceFactory = clientDataServiceFactory;
        }

        public async Task<Client> CreateClient(string name, string contactEmail, string description)
        {
            HelperRepository.ValidatedParameter("Name", name, out name, throwException: true);

            Client client = new Client
            {
                Name = name.ToTitleCase(),
                Secret = $"{Guid.NewGuid():N}".ToUpper(),
                Role = Roles.User,
                AccessTokenLifetimeInMins = Convert.ToInt32(_securitySetting.TokenLifetimeInMins),
                AuthorizationCodeLifetimeInMins = Convert.ToInt32(_securitySetting.CodeLifetimeInMins),
                IsActive = false,
                ContactEmail = contactEmail?.ToLower() ?? default,
                Description = description ?? default
            };

            await _clientDataServiceFactory.InsertOneAsync(client);

            return client;
        }

        public async Task<Client> AuthenticateClient(string apiKey, string appSecret)
        {
            string id = apiKey;

            Client client = await _clientDataServiceFactory.FindByIdAsync(id).ConfigureAwait(false);

            if (!(client != null && client.IsActive && id == client.Id && appSecret == client.Secret)) return null;

            return client;
        }

        public (string token, long expires) CreateAccessToken(Client client)
        {
            //security key for token validation
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySetting.Key));

            //credentials for signing token
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            DateTime baseDate = DateTime.UtcNow;

            Roles role = client.Role;
            string subjectId = client.Id.ToString();
            DateTime expiryDate = baseDate.AddMinutes(client.AccessTokenLifetimeInMins);
            string hashedJti = GenerateJti($"{Guid.NewGuid()}", _securitySetting.Key);

            //add claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, $"{hashedJti}"),
                new Claim(JwtRegisteredClaimNames.Sub, $"{subjectId}"),
                new Claim("cli", $"{client.Id}"),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            //create token
            JwtSecurityToken jwtToken = new JwtSecurityToken(
                issuer: _securitySetting.Issuer,
                audience: _securitySetting.Audience,
                signingCredentials: signingCredentials,
                expires: expiryDate,
                notBefore: baseDate,
                claims: claims);

            string generatedToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return (generatedToken, expiryDate.ToEpoch());
        }

        public async Task<(string token, long expires)> ExtendAccessTokenLifetime(string accessToken, string appSecret)
        {
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadToken(accessToken) as JwtSecurityToken;

            string jti = jwtToken.Claims.First(claim => claim.Type == "jti").Value;
            string sub = jwtToken.Claims.First(claim => claim.Type == "sub").Value;
            Guid cli = Guid.Parse(jwtToken.Claims.First(claim => claim.Type == "cli").Value);

            _ = Convert.FromBase64String(jti);

            Client client = await _clientDataServiceFactory.FindByIdAsync(cli.ToString()).ConfigureAwait(false);

            if (client is null) throw new Exception($"Invalid cli {cli}");
            if (client.Secret != appSecret) throw new Exception($"Invalid appSecret {appSecret}");

            return CreateAccessToken(client);
        }

        public async Task<Client> AssignClientRole(string clientId, Roles role)
        {
            Client client = await _clientDataServiceFactory.FindByIdAsync(clientId).ConfigureAwait(false);

            if (client is null) throw new GenericException($"Client with id '{clientId}' could not be found", "DIRS21001", HttpStatusCode.NotFound);
            if (client.Role == Roles.Root && role != Roles.Root) throw new GenericException("Root role cannot be assigned or revoked", "DIRS21008", HttpStatusCode.Forbidden);

            client.Role = role;
            client.IsActive = true;

            List<(Expression<Func<Client, object>> setExpression, object setFieldValue)> setExpression = new List<(Expression<Func<Client, object>> setExpression, object setFieldValue)>();

            setExpression.Add((x => x.Role, role));
            setExpression.Add((x => x.IsActive, true));

            await _clientDataServiceFactory.UpdateOneAsync(x => x.Id, clientId.ToString(), setExpression);
            return client;
        }

        public async Task<Client> GetClientById(string clientId) => await _clientDataServiceFactory.FindByIdAsync(clientId.ToString()).ConfigureAwait(false);

        public async Task<List<Client>> GetClients() => await _clientDataServiceFactory.FilterBy(_ => true).ConfigureAwait(false);

        public static string GenerateJti(string jti, string key)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] keyBytes = asciiEncoding.GetBytes(key);
            byte[] passwordBytes = asciiEncoding.GetBytes(jti);
            using HMACSHA256 hmacshA256 = new HMACSHA256(keyBytes);
            
            return Convert.ToBase64String(hmacshA256.ComputeHash(passwordBytes));
        }

        public bool ValidateServerKey(string apiKey) => apiKey == _configuration["Events:ConsumerKey"];

        public async Task CreateCollection()
        {
            await _clientDataServiceFactory.CreateCollectionAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> CreateIndexes()
        {
            List<CreateIndexModel<Client>> indexKeysDefine = new List<CreateIndexModel<Client>>()
            {
                new CreateIndexModel<Client>(Builders<Client>.IndexKeys.Ascending(indexKey => indexKey.Role)),
                new CreateIndexModel<Client>(Builders<Client>.IndexKeys.Ascending(indexKey => indexKey.Secret))
            };
           
            return await _clientDataServiceFactory.CreateIndexesAsync(indexKeysDefine).ConfigureAwait(false);
        }

        public async Task<Client> ValidateFindOneAsync(Expression<Func<Client, bool>> filterExpression, bool throwException = false)
        {
            return await _clientDataServiceFactory.ValidateFindOneAsync(filterExpression, throwException: throwException).ConfigureAwait(false);
        }
    }
}
