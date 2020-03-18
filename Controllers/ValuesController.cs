using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace core_cache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        public ValuesController(IMemoryCache cache, IHttpClientFactory clientFactory)
        {
            _cache = cache;
            _clientFactory = clientFactory;
        }

        // GET cached string
        [HttpGet("cache")]
        public async Task<ActionResult> GetCachedValueAsync(string key)
        {
            try {
                var isSet = false;
                var cacheEntry = await _cache.GetOrCreateAsync<List<Post>>("testKey", async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromSeconds(10);
                    isSet = true;
                    var client = _clientFactory.CreateClient();
                    var url = "https://jsonplaceholder.typicode.com/posts";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<Post>>(content);
                        return result;
                    };
                    throw new HttpRequestException($"Bad Request: Invalid URL {url}");
                });
                return Ok(new {
                    cacheEntry = cacheEntry,
                    isSet
                });
            }
            catch (HttpRequestException e) 
            {
                return BadRequest(new {
                    Message = e.Message
                });
            }
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

    public class Post
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
