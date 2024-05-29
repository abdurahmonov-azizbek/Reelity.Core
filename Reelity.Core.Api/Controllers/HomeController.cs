using Microsoft.AspNetCore.Mvc;

namespace Reelity.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString();
        }
    }
}
