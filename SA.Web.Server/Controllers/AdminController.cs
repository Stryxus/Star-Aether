using System;

using Microsoft.AspNetCore.Mvc;
using SA.Web.Server.Data;
using SA.Web.Server.Data.Json;

namespace SA.Web.Server.Controllers
{
    [AdminIPFiltered]
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        public string SendDBUpdateSignal([FromQuery(Name = "u")] string username, [FromQuery(Name = "p")] string password, [FromQuery(Name = "c")] string command)
        {
            if (username == PrivateVariables.Instance.Username && password == PrivateVariables.Instance.Password)
            {
                if (!string.IsNullOrEmpty(command))
                {
                    if (command == "DBUpdate")
                    {
                        Services.Get<MongoDBInterface>().InvokeOnPublicDataUpdate();
                        return "Database update signal sent out!";
                    }
                    else return "Command is not valid!";
                }
                else return "No command specified!";
            }
            else return "Username or Password not valid!";
        }
    }
}
