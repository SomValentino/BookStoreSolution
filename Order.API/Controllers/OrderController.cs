using System.Net;
using AutoMapper;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.API.Application.Queries.GetOrder;
using Order.API.Application.Queries.GetUserOrderHistory;
using Order.API.Dto;

namespace Order.API.Controllers;

[ApiController]
[Route ("[controller]")]
public class OrderController : ControllerBase {
    private readonly IMediator _mediator;
    private readonly ILogger<OrderController> _logger;

    private string UserId => User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Subject) !.Value;

    public OrderController (IMediator mediator,
        ILogger<OrderController> logger) {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet ("{orderId}",Name = "getOrder")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<ActionResult<int>> GetOrder (string orderId) {
        var result = await _mediator.Send (new GetOrderByIdQuery{
            OrderId = orderId
        });
        return Ok (result);
    }

    [HttpGet ("history",Name = "getUserOrderHistory")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<ActionResult<int>> GetUserOrder (int page = 1, int pageSize = 10,
                    DateTime? startDate=null, DateTime? endDate= null) {
        
        var result = await _mediator.Send (new GetUserOrderHistoryQuery{
            UserId = UserId,
            StartDate = startDate,
            EndDate = endDate,
            Page = 1,
            PageSize = pageSize
        });
        return Ok (result);
    }
}