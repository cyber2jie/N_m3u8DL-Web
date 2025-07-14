using System.Reflection;

namespace N_m3u8DL_Web.Util
{


    [AttributeUsage(AttributeTargets.Property)]
    public class BeanField: Attribute
    {
        public string? Name { get; set; }
        public BeanField(string? name)
        {
            Name = name;
        }
    }

    public class BeanUtil
    {
        public static D ToBean<T,D> (T t)
        {
            var dt = typeof(D);

            D d= (D)Activator.CreateInstance(dt);

            if ( t !=null)
            {

                foreach (var item in t.GetType().GetProperties())
                { 
                    var field = dt.GetProperty(item.Name);

                    if (field == null) continue;

                    string fieldName = item.Name;

                    object? value = item.GetValue(t);

                    if (value == null) continue;

                    BeanField beanField= (BeanField)field.GetCustomAttribute(typeof(BeanField));

                    if (beanField != null )
                    {
                        if(beanField.Name != null)
                        {
                            fieldName = beanField.Name;
                        }
                        
                    }


                    PropertyInfo pi = null;

                    foreach(var p in dt.GetProperties())
                    {
                        string pName = p.Name;
                        if (StringUtil.TrimEqual(pName, fieldName))
                        {
                            pi = p;
                            break;
                        }

                    }

                    if (pi != null)
                    {
                        Type valueType = pi.PropertyType;
                        value =ConvertUtil.ConvertValue<object>(value,valueType);
                        pi.SetValue(d,value);

                    }


                }
            }


            return d;

        }
    }
}
