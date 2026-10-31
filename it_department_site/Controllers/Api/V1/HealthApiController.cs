using it_department_site.Infrastructure.Resilience;
using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/health")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class HealthApiController : ControllerBase
    {
        private readonly IResilienceService _resilienceService;
        private static int _simulatedFailuresCount = 0;

        public HealthApiController(IResilienceService resilienceService)
        {
            _resilienceService = resilienceService;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            });
        }

        /// <summary>
        /// Демонстрація відмовостійкості: виконання запиту через Polly (Retry + Circuit Breaker + Timeout)
        /// </summary>
        [HttpGet("resilience-demo")]
        public async Task<IActionResult> TestResilience([FromQuery] bool simulateFailure = false)
        {
            try
            {
                var result = await _resilienceService.ExecuteWithResilienceAsync(async token =>
                {
                    if (simulateFailure)
                    {
                        _simulatedFailuresCount++;
                        if (_simulatedFailuresCount <= 2)
                        {
                            // Імітація тимчасового збою для перевірки Retry
                            throw new HttpRequestException($"Simulated transient network failure #{_simulatedFailuresCount}");
                        }
                    }

                    await Task.Delay(50, token);
                    return "Operation successfully executed via Resilience Pipeline (Polly)";
                });

                return Ok(new
                {
                    status = "Success",
                    message = result,
                    retriesHandled = _simulatedFailuresCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "Faulted",
                    exception = ex.GetType().Name,
                    message = ex.Message
                });
            }
        }
    }
}
