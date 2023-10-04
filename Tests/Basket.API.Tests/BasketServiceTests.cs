using AutoMapper;
using Basket.API.Models.ShoppingCartAggregate;
using Basket.API.Repository;
using Basket.API.Services;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace Basket.API.Tests;

public class BasketServiceTests {
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<ILogger<BasketService>> _loggerMock;

    public BasketServiceTests () {
        _publishMock = new Mock<IPublishEndpoint> ();
        _mapperMock = new Mock<IMapper> ();
        _basketRepositoryMock = new Mock<IBasketRepository> ();
        _loggerMock = new Mock<ILogger<BasketService>> ();

        _basketRepositoryMock.Setup (_ => _.GetBasket ("basketId")).ReturnsAsync (new ShoppingCart ("testId") {
            Items = new List<ShoppingCartItem> {
                new ShoppingCartItem {
                    Quantity = 1,
                        Price = 30,
                        BookId = "BookId",
                        BookTitle = "Book Title",
                }
            }
        });
    }

    [Fact]
    public async Task BasketService_GetBasketWithIdThatExist_ReturnsBasket () {
        var basketService = new BasketService (_basketRepositoryMock.Object, _mapperMock.Object,
            _publishMock.Object, _loggerMock.Object);

        var basket = await basketService.GetBasketAsync ("basketId");

        Assert.True (basket.Items.Count > 0);
    }

    [Fact]
    public async Task BasketService_GetBasketWithIdThatDoesNotExist_ReturnsBasketWithEmptyItems () {
        var basketService = new BasketService (_basketRepositoryMock.Object, _mapperMock.Object,
            _publishMock.Object, _loggerMock.Object);

        var basket = await basketService.GetBasketAsync ("testId");

        Assert.True (basket.Items.Count == 0);
    }

    [Fact]
    public async Task BasketService_UpdateBasketWithIdThatDoesExist_UpdatesBasket () {
        var basketService = new BasketService (_basketRepositoryMock.Object, _mapperMock.Object,
            _publishMock.Object, _loggerMock.Object);

        var shoppingCartItems = new List<ShoppingCartItem> {
            new ShoppingCartItem {
            Quantity = 1,
            Price = 30,
            BookId = "BookId",
            BookTitle = "Book Title",
            },
            new ShoppingCartItem {
            Quantity = 2,
            Price = 40,
            BookId = "Bookv2Id",
            BookTitle = "Bookv2 Title",
            },

        };

        var basket = await basketService.UpdateBasetAsync ("basketId", shoppingCartItems);

        _basketRepositoryMock.Verify (_ => _.UpdateBasket (It.IsAny<ShoppingCart> ()), Times.Once);
    }

    [Fact]
    public async Task BasketService_OnCheckout_PublishesBaskcheckoutEvent () {
        var basketService = new BasketService (_basketRepositoryMock.Object, _mapperMock.Object,
            _publishMock.Object, _loggerMock.Object);

        await basketService.CheckoutAsync ("basketId", "test", "test", "tests", "test@example.com", new ShoppingCart ("basketId") {
            Items = new List<ShoppingCartItem> {
                new ShoppingCartItem {
                    Quantity = 1,
                        Price = 30,
                        BookId = "BookId",
                        BookTitle = "Book Title",
                },
                new ShoppingCartItem {
                    Quantity = 2,
                        Price = 40,
                        BookId = "Bookv2Id",
                        BookTitle = "Bookv2 Title",
                },

            }
        });

        _publishMock.Verify (_ => _.Publish (It.IsAny<BasketCheckoutEvent> (), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task BasketService_AfterBasketCheckoutEventIsPublished_DeletesBasket () {
        var basketService = new BasketService (_basketRepositoryMock.Object, _mapperMock.Object,
            _publishMock.Object, _loggerMock.Object);

        await basketService.CheckoutAsync ("basketId", "test", "test", "tests", "test@example.com", new ShoppingCart ("basketId") {
            Items = new List<ShoppingCartItem> {
                new ShoppingCartItem {
                    Quantity = 1,
                        Price = 30,
                        BookId = "BookId",
                        BookTitle = "Book Title",
                },
                new ShoppingCartItem {
                    Quantity = 2,
                        Price = 40,
                        BookId = "Bookv2Id",
                        BookTitle = "Bookv2 Title",
                },

            }
        });

        _basketRepositoryMock.Verify (_ => _.DeleteBasket("basketId"),Times.Once);
    }

}