using Ardalis.GuardClauses;
using Basket.API.Models.ShoppingCartAggregate;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repository;

public class BasketRepository : IBasketRepository {
    private readonly IDistributedCache _cache;

    public BasketRepository (IDistributedCache distributedCache) {
        _cache = Guard.Against.Null (distributedCache, nameof (distributedCache));
    }
    public async Task DeleteBasket (string userId) {
        await _cache.RemoveAsync (userId);
    }

    public async Task<ShoppingCart?> GetBasket (string userId) {
        var basket = await _cache.GetStringAsync (userId);

        if (string.IsNullOrEmpty (basket))
            return null;

        return JsonConvert.DeserializeObject<ShoppingCart> (basket);
    }

    public async Task<ShoppingCart?> UpdateBasket (ShoppingCart basket) {
        await _cache.SetStringAsync (basket.UserId, JsonConvert.SerializeObject (basket));

        return await GetBasket (basket.UserId);
    }
}