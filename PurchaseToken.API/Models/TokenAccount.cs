using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseToken.API.Models;

public class TokenAccount{
    [BsonId]
    public string UserId { get; private set; }
    public long BookPurchaseToken { get; private set; }

    public TokenAccount(string userId,long bookPurchaseToken = 0)
    {
        UserId = userId;
        BookPurchaseToken = bookPurchaseToken;
    }

    public long Deposit(long bookPurchaseToken){
        BookPurchaseToken += bookPurchaseToken;
        return BookPurchaseToken;
    }

    public long WithDraw(long bookPurchaseToken)
    {
        if (bookPurchaseToken > BookPurchaseToken) return BookPurchaseToken;

        BookPurchaseToken -= bookPurchaseToken;
        return BookPurchaseToken;
    }
}