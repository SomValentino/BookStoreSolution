using Basket.API.Models.ShoppingCartAggregate;

namespace Basket.API.Repository;

public interface IBasketRepository {
    Task<ShoppingCart?> GetBasket (string userId);
    Task<ShoppingCart?> UpdateBasket (ShoppingCart basket);
    Task DeleteBasket (string userId);

}