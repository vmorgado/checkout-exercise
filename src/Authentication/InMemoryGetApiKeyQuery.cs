using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetexample.Authentication
{
    public class InMemoryGetApiKeyQuery : IGetApiKeyQuery
    {
        private readonly IDictionary<string, ApiKey> _apiKeys;

        public InMemoryGetApiKeyQuery()
        {
            var existingApiKeys = new List<ApiKey>
        {
            new ApiKey(1, "market-one", "C5BFF7F0-B4DF-475E-A331-F737424F013C", new DateTime(2020, 01, 01), new List<string> { "Employee" }),
            new ApiKey(2, "store-two", "5908D47C-85D3-4024-8C2B-6EC9464398AD", new DateTime(2020, 01, 01), new List<string> { "Employee" })
        };

            _apiKeys = existingApiKeys.ToDictionary(x => x.Key, x => x);
        }

        public Task<ApiKey> Execute(string providedApiKey)
        {
            _apiKeys.TryGetValue(providedApiKey, out var key);
            return Task.FromResult(key);
        }
    }
}