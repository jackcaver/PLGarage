using GameServer.Models.Config;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers.Common
{
    public class ApiController : Controller
    {
        [HttpGet]
        [Route("api/GetInstanceName")]
        public IActionResult GetInstanceName()
        {
            return Content(ServerConfig.Instance.InstanceName);
        }
    }
}
