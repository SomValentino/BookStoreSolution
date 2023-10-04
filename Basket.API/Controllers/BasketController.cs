using System.Net;
using AutoMapper;
using Basket.API.Models;
using Basket.API.Models.ShoppingCartAggregate;
using Basket.API.Repository;
using Basket.API.Services;
using EventBus.Messages.Events;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;

namespace Basket.API.Controllers;

[ApiController]
[Authorize]
[Route ("[controller]")]
public class BasketController : ControllerBase {
    private readonly IBasketService _basketService;
    private readonly ILogger<BasketController> _logger;

    private string UserId => User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Subject)?.Value ?? string.Empty;

    public BasketController (IBasketService basketService,
        ILogger<BasketController> logger) {
        _basketService = basketService;
        _logger = logger;
    }

    [HttpGet (Name = "GetBasket")]
    [ProducesResponseType (typeof (ShoppingCart), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket () {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        return Ok (await _basketService.GetBasketAsync (UserId));
    }

    [HttpPost]
    [ProducesResponseType (typeof (List<ShoppingCartItem>), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket ([FromBody] List<ShoppingCartItem> items) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        if (items == null || !items.Any ()) return BadRequest ();

        return Ok (await _basketService.UpdateBasetAsync (UserId, items));
    }

    [Route ("[action]")]
    [HttpPost]
    [ProducesResponseType ((int) HttpStatusCode.Accepted)]
    [ProducesResponseType ((int) HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout () {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var basket = await _basketService.GetBasketAsync (UserId);
        if (basket == null) {
            return BadRequest ();
        }
        var UserName = User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.PreferredUserName)?.Value ?? string.Empty;
        var firstName = User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.GivenName)?.Value ?? string.Empty;
        var lastName = User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.FamilyName)?.Value ?? string.Empty;
        var emailAddress = User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Email)?.Value ?? string.Empty;

        await _basketService.CheckoutAsync (UserId, UserName, firstName, lastName, emailAddress, basket);

        return Accepted ();
    }
}