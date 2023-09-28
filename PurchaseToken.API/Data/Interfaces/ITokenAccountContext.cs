using MongoDB.Driver;
using PurchaseToken.API.Models;

namespace PurchaseToken.API.Data.Interfaces;

public interface ITokenAccountContext {
    IMongoCollection<TokenAccount> TokenAccounts { get;}
}