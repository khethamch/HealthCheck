using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var healthCheckResult = await _healthCheckService.CheckHealthAsync();
            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                return Ok(healthCheckResult);
            }
            else if (healthCheckResult.Status == HealthStatus.Degraded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, healthCheckResult);
            }
            else
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, healthCheckResult);
            }
        }
    }
}
