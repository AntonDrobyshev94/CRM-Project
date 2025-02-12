namespace ServicesLibrary.Services.Interfaces
{
    public interface IAuthService
    {
        void AddTokenHeaderMethod(HttpClient httpClient, string token);
        void CheckStatus(HttpResponseMessage response);
        void AddIdempotencyKeyHeader(HttpRequestMessage? request, string idempotencyKey);
    }
}
