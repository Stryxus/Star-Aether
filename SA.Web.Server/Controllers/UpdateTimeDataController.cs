using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace SA.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateTimeDataController : ControllerBase
    {
        [HttpGet]
        public string Get() => Services.Get<MongoDBInterface>().GetUpdateTimesData();
    }
}
