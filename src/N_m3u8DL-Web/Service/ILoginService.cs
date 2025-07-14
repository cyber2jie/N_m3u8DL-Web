using N_m3u8DL_Web.Model;

namespace N_m3u8DL_Web.Service
{
    public interface ILoginService
    {
        public LoginResult Login(LoginForm loginForm);
    }
}
