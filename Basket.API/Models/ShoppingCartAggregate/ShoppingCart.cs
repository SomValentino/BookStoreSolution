namespace Basket.API.Models.ShoppingCartAggregate;

public class ShoppingCart {
    public string UserId { get; set; }
    public List<ShoppingCartItem> Items { get; set; }

    public ShoppingCart (string userId) {
        UserId= userId;
        Items = new List<ShoppingCartItem>();
    }

    public long TotalPrice => Items.Sum (_ => _.Price * _.Quantity);

}