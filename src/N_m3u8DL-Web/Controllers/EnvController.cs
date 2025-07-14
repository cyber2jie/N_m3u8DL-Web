using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Text;

using N_m3u8DL_Web.Util;
namespace N_m3u8DL_Web.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EnvController : ControllerBase
    {
        [HttpGet("list")]
        public IResult Info()
        {
            return Results.Json(Util.OsInfoUtil.Get());
        }
       


    }
}
