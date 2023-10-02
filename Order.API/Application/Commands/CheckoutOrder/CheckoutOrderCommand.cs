using EventBus.Messages.Events;
using MediatR;

namespace Order.API.Application.Commands.CheckoutOrder;

public class CheckoutOrderCommand : BaseOrderCommand, IRequest<Guid>
{
    
}