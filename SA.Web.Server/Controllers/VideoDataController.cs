using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace SA.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoDataController : ControllerBase
    {
        // GET: api/<VideoDataController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<VideoDataController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<VideoDataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<VideoDataController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<VideoDataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
