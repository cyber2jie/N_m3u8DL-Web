using System;
namespace N_m3u8DL_Web.Util
{
    //简单类型转化
    public class ConvertUtil
    {
        public static T ConvertValue<T>(object o,Type valueType) where T:class
        {
            if (o == null )
            {
                return null;
            }

            Type targetType = valueType;

            // 检查是否可以直接转换
            if (targetType.IsAssignableFrom(o.GetType()))
            {
                
                return Convert.ChangeType(o, targetType) as T;
                 
            }

            // 处理常见数据类型的转换
            if (targetType == typeof(string))
            {
                return o.ToString() as T;
            }
            else if (targetType == typeof(int))
            {
                if (int.TryParse(o.ToString(), out int result))
                {
                    return result as T;
                }
            }
            else if (targetType == typeof(double))
            {
                if (double.TryParse(o.ToString(), out double result))
                {
                    return result as T;
                }
            }
            else if (targetType == typeof(decimal))
            {
                if (decimal.TryParse(o.ToString(), out decimal result))
                {
                    return result as T;
                }
            }
            else if (targetType == typeof(DateTime))
            {
                if (DateTime.TryParse(o.ToString(), out DateTime result))
                {
                    return result as T;
                }
            }
            else if (targetType == typeof(bool))
            {
                if (bool.TryParse(o.ToString(), out bool result))
                {
                    return result as T;
                }
            }

            return null;
        }
    }
}
