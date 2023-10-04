using System.Net;
using AutoMapper;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.API.Application.Commands.AuthorizeOrder;
using Order.API.Application.Queries.GetOrder;
using Order.API.Application.Queries.GetUserOrderHistory;
using Order.API.Dto;
using Order.API.Models;

namespace Order.API.Controllers;

[ApiController]
[Route ("[controller]")]
public class OrderController : ControllerBase {
    private readonly IMediator _mediator;
    private readonly ILogger<OrderController> _logger;

    private string UserId => User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Subject)?.Value ?? string.Empty;

    public OrderController (IMediator mediator,
        ILogger<OrderController> logger) {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet ("{orderId}", Name = "getOrder")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<IActionResult> GetOrder (string orderId) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var result = await _mediator.Send (new GetOrderByIdQuery {
            OrderId = orderId
        });
        return Ok (result);
    }

    [HttpGet ("history", Name = "getUserOrderHistory")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserOrder (OrderStatus? orderStatus = null, int page = 1, int pageSize = 10,
        DateTime? startDate = null, DateTime? endDate = null) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var result = await _mediator.Send (new GetUserOrderHistoryQuery {
            UserId = UserId,
                OrderStatus = orderStatus,
                StartDate = startDate,
                EndDate = endDate,
                Page = 1,
                PageSize = pageSize
        });
        return Ok (result);
    }

    [HttpPost ("authorize")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<IActionResult> Authorize ([FromBody] AuthorizeOrderRecord authorizeOrderRecord) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var authorizeOrderCommand = new AuthorizeOrderCommand {
            OrderId = authorizeOrderRecord.OrderId,
            UserId = UserId
        };

        var result = await _mediator.Send (authorizeOrderCommand);

        return Ok (result);
    }
}