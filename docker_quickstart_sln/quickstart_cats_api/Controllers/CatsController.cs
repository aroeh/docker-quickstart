using Microsoft.AspNetCore.Mvc;
using quickstart_lib.Models;

namespace quickstart_cats_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CatsController : ControllerBase
    {
        private readonly ILogger<CatsController> _logger;
        private readonly IEnumerable<Cat> cats;

        public CatsController(ILogger<CatsController> logger)
        {
            _logger = logger;
            cats = new List<Cat>
            {
                new Cat
                {
                    Id = 1,
                    Name = "Smalls",
                    NickNames = new[] { "Mister Smalls", "Big Orange Buddy", "Lord Smalls" }
                },
                new Cat
                {
                    Id = 2,
                    Name = "Scamper",
                    NickNames = new[] { "Scamper Fella", "Little Fella" }
                },
                new Cat
                {
                    Id = 3,
                    Name = "Pepper",
                    NickNames = new[] { "Fluff", "Pepper Buddy", "Pepper Fluff" }
                },
                new Cat
                {
                    Id = 4,
                    Name = "Oreo",
                    NickNames = new[] { "Cookie" }
                }
            };
        }

        [HttpGet(Name = "GetCats")]
        public IActionResult Get()
        {
            _logger.LogInformation("Retrieving Cats");
            return Ok(cats);
        }
    }
}
