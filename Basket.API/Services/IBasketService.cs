using Basket.API.Models.ShoppingCartAggregate;

namespace Basket.API.Services;

public interface IBasketService
{
    Task<ShoppingCart> GetBasketAsync(string UserId);

    Task<ShoppingCart?> UpdateBasetAsync(string UserId, IEnumerable<ShoppingCartItem> items);

    Task CheckoutAsync(string userId, string username, string firstName, 
    string lastName, string emailAddress,ShoppingCart shoppingCart);
}