namespace N_m3u8DL_Web.Model
{
    public class CommonResult:Result
    {
        public string ErrorInfo { get; set;}


        public static CommonResult Success(string info)
        {
            return new CommonResult()
            {
                Message = Constants.ResultSuccess,
                ErrorInfo = info
            };
        }
        public static CommonResult Fail(string info)
        {
            return new CommonResult()
            {
                Message = Constants.ResultFail,
                ErrorInfo = info
            };
        }
    }
}
