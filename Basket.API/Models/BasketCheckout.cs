using Basket.API.Models.ShoppingCartAggregate;

namespace Basket.API.Models;

public class BasketCheckout {
    public string UserName { get; set; }
    public decimal TotalPrice { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public List<ShoppingCartItem> Items { get; set; }
}