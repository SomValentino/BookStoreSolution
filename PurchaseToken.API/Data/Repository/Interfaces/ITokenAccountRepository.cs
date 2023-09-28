using PurchaseToken.API.Models;

namespace PurchaseToken.API.Data.Repository.Interfaces;

public interface ITokenAccountRepository {
    
    Task<TokenAccount> GetUserBalance(string userId);
    Task CreateTokenAccount(TokenAccount account);
    Task<bool> UpdateTokenAccount(TokenAccount account);
}