using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace SA.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadmapDataController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Get() => await Services.Get<MongoDBInterface>().GetRoadmapData();
    }
}
