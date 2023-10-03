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
    private readonly IMapper _mapper;
    private readonly ILogger<OrderController> _logger;

    private string UserId => User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Subject) !.Value;

    public OrderController (IMediator mediator,
        IMapper mapper,
        ILogger<OrderController> logger) {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet (Name = "getOrder")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<ActionResult<int>> GetOrder ([FromBody] GetOrderByIdQuery getOrderByIdQuery) {
        var result = await _mediator.Send (getOrderByIdQuery);
        return Ok (result);
    }

    [HttpGet (Name = "getUserOrderHistory")]
    [ProducesResponseType ((int) HttpStatusCode.OK)]
    public async Task<ActionResult<int>> GetUserOrder ([FromBody] UserOrderHistoryRecord userOrderHistoryRecord) {
        var getUserOrderHistoryQuery = _mapper.Map<GetUserOrderHistoryQuery> (userOrderHistoryRecord);
        var result = await _mediator.Send (getUserOrderHistoryQuery);
        return Ok (result);
    }
}