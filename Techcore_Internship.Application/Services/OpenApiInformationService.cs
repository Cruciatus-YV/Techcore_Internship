namespace Techcore_Internship.Application.Services
{
    public class OpenApiInformationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenApiInformationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetOpenApiInformation()
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://httpbin.org/json");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
