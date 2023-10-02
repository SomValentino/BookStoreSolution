namespace Basket.API.Models.ShoppingCartAggregate;

public class ShoppingCartItem {
    public int Quantity { get; set; }
    public long Price { get; set; }
    public string BookId { get; set; }
    public string BookTitle { get; set; }
}