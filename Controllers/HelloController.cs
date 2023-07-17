using Microsoft.AspNetCore.Mvc;

namespace tic_win1.Controllers;

[ApiController]
[Route("home")]
    public class HelloController : ControllerBase
    {
        [HttpGet("hello")]
        public ActionResult Get()
        {
            var data = new { etna = "Hello World" };
            return Ok(data);
        }
    }