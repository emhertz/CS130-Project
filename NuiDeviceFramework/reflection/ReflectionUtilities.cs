using System;
using System.Reflection;

namespace NuiDeviceFramework.reflection
{
    public static class ReflectionUtilities
    {
        public static object InvokeMethod(object o, String method, object[] parameters)
        {
            if (o == null || method == null)
            {
                return null;
            }
            MethodInfo[] methods = o.GetType().GetMethods();
            foreach (MethodInfo m in methods)
            {
                if (m.Name.Equals(method))
                {
                    object val = m.Invoke(o, parameters);
                    return val;
                }
            }
            return null;
        }

        public static object InvokeProperty(object o, String property)
        {
            if (o == null || property == null)
            {
                return null;
            }
            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                if (p.Name.Equals(property))
                {
                    object val = p.GetValue(o, null);
                    return val;
                }
            }
            return null;
        }
    }
}
