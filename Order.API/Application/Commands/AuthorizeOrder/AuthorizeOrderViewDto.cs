using Order.API.Models;

namespace Order.API.Application.Commands.AuthorizeOrder;

public record AuthorizeOrderViewDto {
    public string OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string ErrorMessage { get; set; }
}