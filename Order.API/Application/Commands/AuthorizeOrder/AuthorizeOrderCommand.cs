
using MediatR;

namespace Order.API.Application.Commands.AuthorizeOrder;


public class AuthorizeOrderCommand : IRequest<AuthorizeOrderViewDto> {
    public string? OrderId { get; set; }
    public string UserId { get; set; }
}