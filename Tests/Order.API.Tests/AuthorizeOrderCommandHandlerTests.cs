using EventBus.Messages.Common;
using EventBus.Messages.Events;
using FluentAssertions;
using Grpc.Core;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Order.API.Application.Commands.AuthorizeOrder;
using Order.API.Data.Repository.Interfaces;
using Order.API.GrpcClient;
using Order.API.Models;
using Order.API.Protos;

namespace Order.API.Tests;

public class AuthorizeOrderCommandHandlerTests {
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<PaymentProtoService.PaymentProtoServiceClient> _paymentProtoGrpcClientServiceMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<AuthorizeOrderCommandHandler>> _logger;
    private readonly Models.Order _pendingOrder;
    private readonly Models.Order _nonpendingOrder;

    public AuthorizeOrderCommandHandlerTests () {
        _orderRepositoryMock = new Mock<IOrderRepository> ();
        _paymentProtoGrpcClientServiceMock = new Mock<PaymentProtoService.PaymentProtoServiceClient> ();
        _publishEndpointMock = new Mock<IPublishEndpoint> ();
        _logger = new Mock<ILogger<AuthorizeOrderCommandHandler>> ();

        _pendingOrder = new Models.Order {
            OrderId = Guid.NewGuid ().ToString (),
            UserId = Guid.NewGuid ().ToString (),
            TotalPrice = 15,
            UserName = "Tests",
            FirstName = string.Empty,
            LastName = string.Empty,
            OrderStatus = OrderStatus.Pending,
            EmailAddress = "test@example.com",
            Items = new List<EventBus.Messages.Events.ShoppingItem> {
            new EventBus.Messages.Events.ShoppingItem {
            BookId = "BookId",
            BookTitle = "BookTitle",
            Quantity = 5,
            Price = 3
            }
            }
        };

        _nonpendingOrder = new Models.Order {
            OrderId = Guid.NewGuid ().ToString (),
            UserId = Guid.NewGuid ().ToString (),
            TotalPrice = 15,
            UserName = "Tests",
            FirstName = string.Empty,
            LastName = string.Empty,
            OrderStatus = OrderStatus.Cancelled,
            EmailAddress = "test@example.com",
            Items = new List<EventBus.Messages.Events.ShoppingItem> {
            new EventBus.Messages.Events.ShoppingItem {
            BookId = "BookId",
            BookTitle = "BookTitle",
            Quantity = 5,
            Price = 3
            }
            }
        };
    }

    [Fact]
    public async Task AuthorizeOrderCommandHandler_OnSuccessFulAuthoriztion_ConfirmsOrder () {
        _orderRepositoryMock.Setup (_ => _.GetOrderById (_pendingOrder.OrderId)).ReturnsAsync (_pendingOrder);

        _paymentProtoGrpcClientServiceMock.Setup (_ => _.AuthorizePaymentAsync (new PaymentRequest {
            UserId = _pendingOrder.UserId,
                Amount = _pendingOrder.TotalPrice
        }, It.IsAny<Metadata> (), null, CancellationToken.None)).Returns (new Grpc.Core.AsyncUnaryCall<PaymentResponse> (Task.FromResult (new PaymentResponse {
            Status = true
        }), null!, null!, null!, null!));

        var authorizeOrderCommandHandler = new AuthorizeOrderCommandHandler (_orderRepositoryMock.Object,
            new PaymentGrpcClientService (_paymentProtoGrpcClientServiceMock.Object), _publishEndpointMock.Object, _logger.Object);

        var result = await authorizeOrderCommandHandler.Handle (new AuthorizeOrderCommand {
            OrderId = _pendingOrder.OrderId,
                CorrelationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        }, CancellationToken.None);

        result.OrderStatus.Should ().Be (OrderStatus.Confirmed);
    }

    [Fact]
    public async Task AuthorizeOrderCommandHandler_OnSuccessFulAuthoriztion_PublishOrderStatusConfirmedEvent () {
        _orderRepositoryMock.Setup (_ => _.GetOrderById (_pendingOrder.OrderId)).ReturnsAsync (_pendingOrder);

        _paymentProtoGrpcClientServiceMock.Setup (_ => _.AuthorizePaymentAsync (new PaymentRequest {
            UserId = _pendingOrder.UserId,
                Amount = _pendingOrder.TotalPrice
        }, It.IsAny<Metadata> (), null, CancellationToken.None)).Returns (new Grpc.Core.AsyncUnaryCall<PaymentResponse> (Task.FromResult (new PaymentResponse {
            Status = true
        }), null!, null!, null!, null!));

        var authorizeOrderCommandHandler = new AuthorizeOrderCommandHandler (_orderRepositoryMock.Object,
            new PaymentGrpcClientService (_paymentProtoGrpcClientServiceMock.Object), _publishEndpointMock.Object, _logger.Object);

        var result = await authorizeOrderCommandHandler.Handle (new AuthorizeOrderCommand {
            OrderId = _pendingOrder.OrderId,
                CorrelationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        }, CancellationToken.None);

        _publishEndpointMock.Verify (_ => _.Publish (It.IsAny<OrderStatusConfirmedEvent> (), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AuthorizeOrderCommandHandler_OnFailedAuthoriztion_FailsOrder () {
        _orderRepositoryMock.Setup (_ => _.GetOrderById (_pendingOrder.OrderId)).ReturnsAsync (_pendingOrder);

        _paymentProtoGrpcClientServiceMock.Setup (_ => _.AuthorizePaymentAsync (new PaymentRequest {
            UserId = _pendingOrder.UserId,
                Amount = _pendingOrder.TotalPrice
        }, It.IsAny<Metadata> (), null, CancellationToken.None)).Returns (new Grpc.Core.AsyncUnaryCall<PaymentResponse> (Task.FromResult (new PaymentResponse {
            Status = false
        }), null!, null!, null!, null!));

        var authorizeOrderCommandHandler = new AuthorizeOrderCommandHandler (_orderRepositoryMock.Object,
            new PaymentGrpcClientService (_paymentProtoGrpcClientServiceMock.Object), _publishEndpointMock.Object, _logger.Object);

        var result = await authorizeOrderCommandHandler.Handle (new AuthorizeOrderCommand {
            OrderId = _pendingOrder.OrderId,
                CorrelationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        }, CancellationToken.None);

        result.OrderStatus.Should ().Be (OrderStatus.Failed);
    }

    [Fact]
    public async Task AuthorizeOrderCommandHandler_OnAuthoriztionOfNonPendingOrder_ReturnsErrorMessage () {
        _orderRepositoryMock.Setup (_ => _.GetOrderById (_nonpendingOrder.OrderId)).ReturnsAsync (_nonpendingOrder);

        _paymentProtoGrpcClientServiceMock.Setup (_ => _.AuthorizePaymentAsync (new PaymentRequest {
            UserId = _nonpendingOrder.UserId,
                Amount = _nonpendingOrder.TotalPrice
        }, null, null, CancellationToken.None)).Returns (new Grpc.Core.AsyncUnaryCall<PaymentResponse> (Task.FromResult (new PaymentResponse {
            Status = true
        }), null!, null!, null!, null!));

        var authorizeOrderCommandHandler = new AuthorizeOrderCommandHandler (_orderRepositoryMock.Object,
            new PaymentGrpcClientService (_paymentProtoGrpcClientServiceMock.Object), _publishEndpointMock.Object, _logger.Object);

        var result = await authorizeOrderCommandHandler.Handle (new AuthorizeOrderCommand {
            OrderId = _nonpendingOrder.OrderId,
                CorrelationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        }, CancellationToken.None);

        result.ErrorMessage.Should ().NotBeNullOrEmpty ();
        result.ErrorMessage.Should ().Be ("Only pending orders can be authorized for payment");
    }
}