using System;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.gestures.implementations;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.managers;
using NuiDeviceFramework.reflection;
using System.Collections.Generic;

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

            GestureManager gm = new GestureManager(device);
            Gesture swipeLeft = new SwipeLeft(device);

            if (!gm.Add(swipeLeft))
            {
                Console.WriteLine("Could not add the gesture {0} to the device {1}. Unsupported gesture.", swipeLeft, device);
                Environment.Exit(-1);
            }

            Console.WriteLine("You've successfully added a Gesture to the GestureManager!");

            Console.WriteLine("Now the GestureManager will start listening for input.");
            gm.Start();
            List<Gesture> completedGestures;
            for (; ; )
            {
                completedGestures = gm.getCompletedGestures();
                if (completedGestures.Count == 0)
                {
                    Console.WriteLine("No gesture detected this frame.");
                }
                else
                {
                    break;
                }
            }

            foreach (Gesture ge in completedGestures)
            {
                Console.WriteLine("Gesture {0} successfully detected!", ge);
            }

            Console.WriteLine("The program will now exit.");
        }
    }
}
