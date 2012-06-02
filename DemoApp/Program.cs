using System;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.managers;
using NuiDeviceFramework.reflection;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Cisco Kinect Team 1 Demo Application.");
            Console.WriteLine("First, we will set up a connection to the Kinect.");
            String deviceName = "NuiDeviceFramework.devices.Kinect";
            String dllPath = "C:\\Users\\Eric\\Documents\\Visual Studio 2010\\Projects\\NuiDeviceFramework\\NuiDeviceFramework\\bin\\Debug\\NuiDeviceFramework.dll";
            object device = DeviceManager.GetConnection(deviceName,dllPath);

            if (device == null)
            {
                Console.WriteLine("Error occurred connecting to the {0} device.", deviceName);
                Environment.Exit(-1);
            }
           
            NuiStreamTypes s = NuiStreamTypes.ColorData;
            for (; s <= NuiStreamTypes.AudioData; s++)
            {
                object val = ReflectionUtilities.InvokeMethod(device, "supportsStreamType", new object[]{s});
                if (val is bool && (bool)val == true)
                {
                    Console.WriteLine("Device {0} supports stream type {1}", deviceName, s);
                }
            }
        }
    }
}
