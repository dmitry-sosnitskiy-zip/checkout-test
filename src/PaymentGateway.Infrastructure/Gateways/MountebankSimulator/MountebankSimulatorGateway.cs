using System.Text.Json;

using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Gateways;
using PaymentGateway.Infrastructure.Gateways.MountebankSimulator.DataContracts;

namespace PaymentGateway.Infrastructure.Gateways.MountebankSimulator;

public class MountebankSimulatorGateway(IHttpClient httpClient) : PaymentProcessingGateway
{
    public const string MakePaymentApiEndpoint = $"http://localhost:8080/payments";

    public override GatewayType GatewayType => GatewayType.MountebankSimulator;

    protected override async Task<GatewayResult> ProcessPaymentInternal(Payment payment, GatewayCardData cardData)
    {
        var request = new ProcessPaymentRequest(
            CardNumber: cardData.CardNumber,
            ExpiryDate: $"{cardData.ExpDateMonth:D2}/{cardData.ExpDateYear:D2}",
            Currency: payment.CurrencyCode,
            Amount: payment.Amount,
            Cvv: cardData.Cvv
        );
        var response = await httpClient.SendPostRequest<ProcessPaymentResponse>(MakePaymentApiEndpoint, request);

        var result = new GatewayResult(Success: response.Authorized,
            GatewayMessage: JsonSerializer.Serialize(response),
            GatewayReferenceId: response.AuthorizationCode);

        return result;
    }
}