using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N_m3u8DL_Web.Service;
using N_m3u8DL_Web.Service.Impl;

namespace N_m3u8DL_Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private  readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public IResult Login([FromBody] Model.LoginForm loginForm)
        {
            return Results.Json(_loginService.Login(loginForm));
        }
    }
}
