using BookStore.Helpers.Features;
using EventBus.Messages.Common;
using Grpc.Core;
using Purchase.API.Protos;
using PurchaseToken.API.Data.Repository.Interfaces;

namespace PurchaseToken.API.GrpcService;

public class PaymentGrpcService : PaymentProtoService.PaymentProtoServiceBase {
    private readonly ITokenAccountRepository _tokenAccountRepository;
    private readonly ILogger<PaymentGrpcService> _logger;

    public PaymentGrpcService (ITokenAccountRepository tokenAccountRepository,
        ILogger<PaymentGrpcService> logger) {
        _tokenAccountRepository = tokenAccountRepository;
        _logger = logger;
    }

    public override async Task<PaymentResponse> AuthorizePayment (PaymentRequest request, ServerCallContext context) {
        try {
            if (context.RequestHeaders != null) {
                var correlationId = context.RequestHeaders.GetValue (EventBusConstants._correlationIdHeader);
                _logger.LogInformation ("Handling Grpc request with correlationId: {id}", correlationId);
            }
            _logger.LogInformation ("Fetching token account for user with Id: {id}", request.UserId);
            var tokenAccount = await _tokenAccountRepository.GetUserBalance (request.UserId);

            if (tokenAccount == null) {
                return new PaymentResponse {
                Status = false,
                ErrorMessage = "Account not found"
                };
            }

            if (request.Amount > tokenAccount.BookPurchaseToken) {
                return new PaymentResponse {
                    Status = false,
                        ErrorMessage = "Insufficent funds"
                };
            }

            tokenAccount.WithDraw (request.Amount);

            await _tokenAccountRepository.UpdateTokenAccount (tokenAccount);

            _logger.LogInformation ("Successfully completed payment authorization");

            return new PaymentResponse {
                Status = true
            };

        } catch (System.Exception ex) {

            _logger.LogError (ex, ex.Message);
            return new PaymentResponse {
                Status = false,
                    ErrorMessage = "System Error"
            };
        }
    }
}