using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using quickstart_lib.Models;
using StackExchange.Redis;

namespace quickstart_cats_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CatsController : ControllerBase
    {
        private readonly ILogger<CatsController> _logger;
        private List<Cat> cats;
        private readonly IConnectionMultiplexer cache;
        private readonly IDatabase cacheDb;

        public CatsController(ILogger<CatsController> logger, IConnectionMultiplexer cacheConnection)
        {
            _logger = logger;
            cache = cacheConnection;
            cacheDb = cache.GetDatabase();

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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation("checking cache for value");
            var cacheString = cacheDb.StringGet($"cat:{id}");
            if (!string.IsNullOrWhiteSpace(cacheString))
            {
                _logger.LogInformation("data found in cache.  returning cached value");
                var cacheVal = JsonConvert.DeserializeObject<Cat>(cacheString);
                return Ok(cacheVal);
            }

            _logger.LogInformation("data not found in cache");
            _logger.LogInformation("retrieving data from repository");

            var cat = cats.First(c => c.Id == id);

            _logger.LogInformation("data found...adding to cache");
            cacheDb.StringSet($"cat:{cat.Id}", JsonConvert.SerializeObject(cat));

            return Ok(cat);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Cat cat)
        {
            //add cat to the existing data
            cat.Id = cats.Count + 1;
            cats.Add(cat);

            _logger.LogInformation("checking cache for value");
            if (cacheDb.KeyExists($"cat:{cat.Id}"))
            {
                _logger.LogInformation("value exists in cache");
                _logger.LogInformation("deleting value from cache");
                cacheDb.StringGetDelete($"cat:{cat.Id}");
            }

            _logger.LogInformation("adding updated value to cache");
            cacheDb.StringSet($"cat:{cat.Id}", JsonConvert.SerializeObject(cat));

            return Ok("new cat added");
        }
    }
}
