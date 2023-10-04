using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Order.API.Application.Commands.CheckoutOrder;
using Order.API.Data.Repository.Interfaces;

namespace Order.API.Tests;

public class CheckoutOrderCommandHandlerTests {
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ILogger<CheckoutOrderCommandHandler>> _logger;

    public CheckoutOrderCommandHandlerTests () {
        _orderRepositoryMock = new Mock<IOrderRepository> ();
        _logger = new Mock<ILogger<CheckoutOrderCommandHandler>> ();
    }

    [Fact]
    public async Task CheckoutOrderCommandHandler_HandleCheckoutOrder_CreatesOrder () {
        var checkoutOrderCommandHandler = new CheckoutOrderCommandHandler (_orderRepositoryMock.Object, _logger.Object);

        var orderId =await checkoutOrderCommandHandler.Handle(new CheckoutOrderCommand {
            UserId = "TestId",
            UserName = "Tests",
            FirstName = "Tests",
            LastName = "Tests",
            EmailAddress = "test@example.com",
            Items = new List<EventBus.Messages.Events.ShoppingItem>{
                new EventBus.Messages.Events.ShoppingItem{
                    BookId = "BookId",
                    BookTitle = "BookTitle",
                    Price = 55,
                    Quantity = 45
                }
            }
        },CancellationToken.None);

        _orderRepositoryMock.Verify(_ => _.CreateOrder(It.IsAny<Models.Order>()),Times.Once);
        orderId.Should().NotBeNullOrEmpty();
    }
}