using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Uri> _serverUrls = new List<Uri>();

            var consulClient = new ConsulClient(c => c.Address = new Uri("http://consul:8500"));
            var services = consulClient.Agent.Services().Result.Response;
            foreach (var service in services)
            {
                var isTestApi = service.Value.Tags.Any(t => t == "Test") ||
                                  service.Value.Tags.Any(t => t == "Prova");
                if (isTestApi)
                {
                    var serviceUri = new Uri($"{service.Value.Address}:{service.Value.Port}");
                    _serverUrls.Add(serviceUri);
                }
            }

            if (_serverUrls.Count == 0)
                return Ok("No services available");

            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync($"{_serverUrls.First()}api/values");

                return Ok($"{result.StatusCode}: '{await result.Content.ReadAsStringAsync()}'");
            }
        }
    }
}
