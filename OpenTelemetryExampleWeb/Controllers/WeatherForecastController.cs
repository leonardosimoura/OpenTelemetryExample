using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenTelemetryExampleWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = new ConcurrentBag<IEnumerable<WeatherForecast>>();

            var tasks = Enumerable.Range(0, 100).Select(s => GetData()).ToArray();

            Task.WaitAll(tasks);

            return Ok(tasks.SelectMany(s => s.Result));

        }

        private async Task<IEnumerable<WeatherForecast>> GetData()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(@"http://localhost:5051/WeatherForecast");
                    return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception)
            {
                return Enumerable.Empty<WeatherForecast>();
            }
        }
    }
}
