using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ApplicationOptions Options { get; set; }

        public ValuesController(IOptionsSnapshot<ApplicationOptions> options)
        {
            Options = options.Value;
        }
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            string m = Options.Age < 18 ? "minorenne":"maggiorenne";
            return Ok($"Ciao {Options.Name} sei {m}!");
        }
    }
}
