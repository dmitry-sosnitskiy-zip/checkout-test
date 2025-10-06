namespace PaymentGateway.Domain;

public interface IHttpClient
{
    Task<TResponse> SendPostRequest<TResponse>(string address, object requestBody);
}