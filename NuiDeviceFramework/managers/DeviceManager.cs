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
        private static NuiDevice device;

        public static NuiDevice GetConnection(String deviceName, String dll)
        {
            Assembly assembly = Assembly.LoadFile(dll);
            Type type = assembly.GetType(deviceName);

            device = (NuiDevice)Activator.CreateInstance(type);
            return device;
        }
    }
}
