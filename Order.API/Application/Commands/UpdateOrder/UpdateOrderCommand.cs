using MediatR;

namespace Order.API.Application.Commands.UpdateOrder;

public class UpdateOrderCommand : BaseOrderCommand, IRequest {
    public Guid OrderId { get; set; }
}