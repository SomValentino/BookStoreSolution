using MongoDB.Driver;
using PurchaseToken.API.Data.Interfaces;
using PurchaseToken.API.Models;

namespace PurchaseToken.API.Data;

public class TokenAccountContext : ITokenAccountContext {
    public TokenAccountContext (IConfiguration configuration) {
        var client = new MongoClient (configuration.GetValue<string> ("ConnectionString"));
        var database = client.GetDatabase (configuration.GetValue<string> ("DatabaseName"));

        TokenAccounts = database.GetCollection<TokenAccount> (configuration.GetValue<string> ("CollectionName"));
    }
    public IMongoCollection<TokenAccount> TokenAccounts { get; }
}