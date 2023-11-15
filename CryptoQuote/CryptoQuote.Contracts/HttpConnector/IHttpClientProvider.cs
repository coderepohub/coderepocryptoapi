namespace CryptoQuote.Contracts.HttpConnector;

public interface IHttpClientProvider
{
    /// <summary>
    /// Make httpclient get call.
    /// </summary>
    /// <param name="uri">uri path of the api.</param>
    /// <returns>Http Response Message.</returns>
    Task<HttpResponseMessage> GetJsonAsync(string uri);

}
