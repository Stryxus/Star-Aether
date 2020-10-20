using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace SA.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangelogPostDataController : ControllerBase
    {
        // GET: api/<ChangelogPostDataController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ChangelogPostDataController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ChangelogPostDataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ChangelogPostDataController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ChangelogPostDataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
