using System;
using System.Reflection;

namespace NuiDeviceFramework.reflection
{
    public static class ReflectionUtilities
    {
        public static object InvokeMethod(object o, String method, object[] parameters)
        {
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
    }
}
