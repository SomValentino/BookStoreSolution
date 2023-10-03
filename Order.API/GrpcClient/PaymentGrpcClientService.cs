using Order.API.Protos;

namespace Order.API.GrpcClient;

public class PaymentGrpcClientService 
{
    private readonly PaymentProtoService.PaymentProtoServiceClient _paymentProtoServiceClient;

    public PaymentGrpcClientService(PaymentProtoService.PaymentProtoServiceClient paymentProtoServiceClient)
    {
        _paymentProtoServiceClient = paymentProtoServiceClient;
    }
    public async Task<PaymentResponse> Authorize(PaymentRequest paymentRequest)
    {
        var paymentResponse = await _paymentProtoServiceClient.AuthorizePaymentAsync(paymentRequest);

        return paymentResponse;
    }
}