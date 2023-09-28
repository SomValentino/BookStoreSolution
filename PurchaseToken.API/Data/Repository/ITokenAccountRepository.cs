using MongoDB.Driver;
using PurchaseToken.API.Data.Interfaces;
using PurchaseToken.API.Data.Repository.Interfaces;
using PurchaseToken.API.Models;

namespace PurchaseToken.API.Data.Repository;

public class TokenAccountRepository : ITokenAccountRepository {
    private readonly ITokenAccountContext _tokenAccountContext;

    public TokenAccountRepository (ITokenAccountContext tokenAccountContext) {
        _tokenAccountContext = tokenAccountContext;
    }

    public async Task CreateTokenAccount (TokenAccount account) {
        await _tokenAccountContext.TokenAccounts.InsertOneAsync (account);
    }

    public async Task<TokenAccount> GetUserBalance (string userId) {
        return await _tokenAccountContext.TokenAccounts.Find (_ => _.UserId == userId).FirstOrDefaultAsync ();
    }

    public async Task<bool> UpdateTokenAccount (TokenAccount account) {
        var updateResult = await _tokenAccountContext
            .TokenAccounts
            .ReplaceOneAsync (filter: g => g.UserId == account.UserId, replacement: account);

        return updateResult.IsAcknowledged &&
            updateResult.ModifiedCount > 0;
    }
}