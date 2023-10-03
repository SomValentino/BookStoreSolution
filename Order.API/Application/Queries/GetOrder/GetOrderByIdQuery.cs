using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Order.API.Application.Queries.GetOrder;

public class GetOrderByIdQuery: IRequest<OrderViewDto> 
{
    [Required]
    public Guid OrderId { get; set; }
}