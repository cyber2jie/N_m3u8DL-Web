using N_m3u8DL_Web.Model;

using N_m3u8DL_Web.Context;
using JWT.Builder;
using JWT.Algorithms;

namespace N_m3u8DL_Web.Service.Impl
{


    public record UserInfo
    {
        public string User { get; set; }
        public string LoginId { get; set; }

        public long ExpireTime { get; set; }
    }

    public class ValidateCache:Dictionary<string,UserInfo>
    {
       public static ValidateCache Instance=new ValidateCache();
    }

    public class LoginService:ILoginService
    {

        public LoginResult Login(LoginForm loginForm)
        {
            LoginResult result = new LoginResult();

            Config.Config config = Context.ContextHolder.Instance.Context.Config;

            result.Message = Constants.ResultFail;

            if (config.Admin.User.Equals(loginForm.User)
                 && config.Admin.Password.Equals(loginForm.Password)
                )
            {


                result.Message=Constants.ResultSuccess;


                string loginId=Guid.NewGuid().ToString();
                long timeSeconds=DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();

                ValidateCache.Instance.Add(loginId,new UserInfo()
                {
                    User=loginForm.User,
                    LoginId=loginId,
                    ExpireTime=timeSeconds
                });


                string token=JwtBuilder.Create()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(config.Admin.Certificate)
                    .AddClaim("user", loginForm.User)
                    .AddClaim("expire",timeSeconds)
                    .AddClaim("loginId", loginId)
                    .Subject("N_m3u8DL-Web").Encode();

                result.Access_Token = token;


            }
            return result;

        }
         
       
    }
}
