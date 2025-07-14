using System.Reflection;

namespace N_m3u8DL_Web.Util
{

    public class RequireException:Exception
    {
        public RequireException(string message):base(message)
        {
        }
    }
    public class FormRequireUtil
    {
        public static void Require(object o ,params string []fields)
        {
            PropertyInfo[] propertyInfos= o.GetType().GetProperties(); 
            foreach (var field in fields)
            {
                PropertyInfo property=propertyInfos.Where(x=>StringUtil.TrimEqual(field,x.Name)).First();
                
                if (property != null)
                {
                    object val= property.GetValue(o);

                    if (val == null)
                    {
                        throw new RequireException($"{field} is required ");
                        
                    }

                    if( val.GetType() == typeof(string))
                    {

                        if (String.IsNullOrWhiteSpace((string)val))
                        {
                            throw new RequireException($"{field} is required ");
                        }

                    }

                }

            }

        }
    }
}
