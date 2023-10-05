using System.Net;
using AutoMapper;
using BookStore.Helpers.Interfaces;
using EventBus.Messages.Common;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.API.Application.Commands.AuthorizeOrder;
using Order.API.Application.Commands.CancelOrder;
using Order.API.Application.Queries.GetOrder;
using Order.API.Application.Queries.GetUserOrderHistory;
using Order.API.Dto;
using Order.API.Models;

namespace Order.API.Controllers;

[ApiController]
[Route ("[controller]")]
public class OrderController : ControllerBase {
    private readonly IMediator _mediator;
    private readonly ICorrelationGenerator _correlationGenerator;
    private readonly ILogger<OrderController> _logger;

    private string UserId => User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Subject)?.Value ?? string.Empty;

    public OrderController (IMediator mediator,
        ILogger<OrderController> logger,
        ICorrelationGenerator correlationGenerator) {
        _mediator = mediator;
        _correlationGenerator = correlationGenerator;
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
    public async Task<IActionResult> GetUserOrder ([FromQuery] UserOrderHistoryRecord userOrderHistoryRecord) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var result = await _mediator.Send (new GetUserOrderHistoryQuery {
            UserId = UserId,
            OrderStatus = userOrderHistoryRecord.OrderStatus,
            StartDate = userOrderHistoryRecord.StartDate,
            EndDate = userOrderHistoryRecord.EndDate,
            Page = userOrderHistoryRecord.Page,
            PageSize = userOrderHistoryRecord.PageSize
        });
        return Ok (result);
    }

    [HttpPost ("cancel")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<IActionResult> Cancel ([FromBody] CancelOrderCommand cancelOrderCommand) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();

        var result = await _mediator.Send (cancelOrderCommand);

        return NoContent ();
    }

    [HttpPost ("authorize")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<IActionResult> Authorize ([FromBody] AuthorizeOrderRecord authorizeOrderRecord) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var correlationId = _correlationGenerator.Get ();
        var authorizeOrderCommand = new AuthorizeOrderCommand {
            OrderId = authorizeOrderRecord.OrderId,
            UserId = UserId,
            CorrelationId = correlationId
        };

        var result = await _mediator.Send (authorizeOrderCommand);

        return Ok (result);
    }
}