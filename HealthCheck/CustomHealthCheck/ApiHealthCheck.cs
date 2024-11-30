using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace HealthCheck.CustomHealthCheck
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiHealthCheck> _logger;

        public ApiHealthCheck(IHttpClientFactory httpClientFactory, ILogger<ApiHealthCheck> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var startTime = DateTime.UtcNow;
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = await httpClient.GetAsync("https://localhost:7110/WeatherForecast", cancellationToken);

                var duration = DateTime.UtcNow - startTime;

                if (response.IsSuccessStatusCode)
                {
                    var healthCheckData = new Dictionary<string, object>
{
    { "Name", context.Registration.Name },  
    { "Tags", string.Join(",", context.Registration.Tags) },  
    { "StatusCode", response.StatusCode },
    { "Duration", duration.TotalMilliseconds },
    { "Health", "Healthy" },
    { "Description", "The API responded successfully with the expected status code." }
};

                    return HealthCheckResult.Healthy($"Api returned status code {response.StatusCode}", healthCheckData);
                }
                else
                {
                    _logger.LogWarning("Health check failed. Status code: {StatusCode}", response.StatusCode);
                    return HealthCheckResult.Unhealthy($"API returned status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed due to an exception.");
                return HealthCheckResult.Unhealthy("Health check failed due to an exception", ex);
            }
        }
    }
}
