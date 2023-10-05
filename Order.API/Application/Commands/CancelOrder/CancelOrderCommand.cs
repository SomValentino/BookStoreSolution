

using MediatR;

namespace Order.API.Application.Commands.CancelOrder;

public class CancelOrderCommand : IRequest {
    public string OrderId { get; set; }
}