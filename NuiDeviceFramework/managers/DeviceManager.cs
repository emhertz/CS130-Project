using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuiDeviceFramework.devices;
using System.Reflection;

namespace NuiDeviceFramework.managers
{
    public static class DeviceManager
    {
        private static object deviceObject;

        public static object GetConnection(String deviceName, String dll)
        {
            //Assembly assembly = Assembly.LoadFile(dll);
            Assembly assembly = Assembly.LoadFrom(dll);
            Type[] types = assembly.GetTypes();
            Type type = null;

            foreach (Type t in types)
            {
                if (t.FullName.Equals(deviceName))
                {
                    type = t;
                    break;
                }
            }

            deviceObject = Activator.CreateInstance(type);
            return deviceObject;
        }
    }
}
