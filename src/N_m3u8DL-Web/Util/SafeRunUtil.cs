using SqlSugar;

namespace N_m3u8DL_Web.Util
{



    public delegate T? GetVal<T>();
    public delegate T? GetValByException<T>(Exception e);

    public class SafeRun<T>
    {
        public Exception? Exception { get; set; }

        public object? Result { get; set; }

        public GetValByException<T>? GetValByException { get; set; }

        public SafeRun<T> Error (GetValByException<T> getVal)
        {
            this.GetValByException = getVal;

            return this;
        }

        public T Get()
        {
            if (Exception != null)
            {
                if (GetValByException != null)
                {
                   return GetValByException.Invoke(Exception);
                }
            }
            return (T)Result;

        }



    }
    public class SafeRunUtil
    {



        public static SafeRun<T> Run<T> (GetVal<T> getVal)
        {
            Exception ex = null;
            T result = default(T);
            try
            {
                result=getVal.Invoke();
            }
            catch (Exception e)
            {
                ex = e;
            }

            return new SafeRun<T>()
            {
                Exception = ex,
                Result = result
            };

        }

    }
}
