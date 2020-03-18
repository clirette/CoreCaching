using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace core_cache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly PasswordHasher<string> _hasher;

        public ValuesController(IMemoryCache cache)
        {
            _cache = cache;
            _hasher = new PasswordHasher<string>();
        }

        // GET cached string
        [HttpGet("cache")]
        public async Task<ActionResult<dynamic>> GetCachedValueAsync(string key)
        {
            var isSet = false;
            var cacheEntry = await _cache.GetOrCreateAsync("testKey", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(10);
                isSet = true;
                var hash = _hasher.HashPassword("testing", "password123password123password123password123password123password123");
                return Task.FromResult(hash);
            });
            return Ok(new {
                cacheEntry,
                isSet
            });
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
