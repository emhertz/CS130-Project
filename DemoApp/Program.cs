/**************************************************************
 This file is part of Kinect Sensor Architecture Development Project.

   Kinect Sensor Architecture Development Project is free software:
   you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   Kinect Sensor Architecture Development Project is distributed in
   the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Kinect Sensor Architecture Development Project.  If
   not, see <http://www.gnu.org/licenses/>.
**************************************************************/
/**************************************************************
The work was done in joint collaboration with Cisco Systems Inc.
Copyright ｩ 2012, Cisco Systems, Inc. and UCLA
*************************************************************/

using System;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.gestures.implementations;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.managers;
using NuiDeviceFramework.reflection;
using System.Collections.Generic;
using System.Messaging;

namespace DemoApp
{
    class Program
    {
        static void CreateQueue(string queuePath)
        {
            try
            {
                if (!MessageQueue.Exists(queuePath))
                {
                    MessageQueue.Create(queuePath);
                }
                else
                {
                    Console.WriteLine(queuePath + " already exists.");
                }
            }
            catch (MessageQueueException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void SendMessage(String queuePath, String message)
        {
            try
            {
                // Connect to a queue on the local computer.
                MessageQueue myQueue = new MessageQueue(queuePath);

                Message myMessage = new Message(message, new BinaryMessageFormatter());

                // Send the image to the queue.
                myQueue.Send(myMessage);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);

            }
            return;
        }

        public static void ReceiveMessage(string queuePath)
        {
            try
            {
                // Connect to the a queue on the local computer.
                MessageQueue myQueue = new MessageQueue(queuePath);

                // Receive and format the message. 
                System.Messaging.Message myMessage = myQueue.Receive();
                myMessage.Formatter = new BinaryMessageFormatter();
                string message = (string)myMessage.Body;

                Console.WriteLine("message received: {0}", message);

                switch (message)
                {
                    case "":
                        break;
                }
            }

            catch (MessageQueueException)
            {
                // Handle Message Queuing exceptions.
            }

            // Handle invalid serialization format.
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            
            return;
        }


        static void RunMessagingApplication()
        {
            Console.WriteLine("Welcome to the Cisco Kinect Team 1 Demo Application (Messaging Version).");
            Console.WriteLine("First, we will set up a connection to the Kinect.");
            string recvQueuePath = ".\\Private$\\sendQueue";
            string sendQueuePath = ".\\Private$\\recvQueue";
            CreateQueue(recvQueuePath);
            CreateQueue(sendQueuePath);
            SendMessage(sendQueuePath, "Kinect");
            ReceiveMessage(recvQueuePath);
            SendMessage(sendQueuePath, "AudioGesture");
            ReceiveMessage(recvQueuePath);
            ReceiveMessage(recvQueuePath);
        }

        static void RunStandaloneTestApplication()
        {
            Console.WriteLine("Welcome to the Cisco Kinect Team 1 Demo Application.");
            Console.WriteLine("First, we will set up a connection to the Kinect.");
            String deviceName = "NuiDeviceFramework.devices.Kinect";
            String dllPath = "NuiDeviceFramework.dll";
            object device = DeviceManager.GetConnection(deviceName, dllPath);

            if (device == null)
            {
                Console.WriteLine("Error occurred connecting to the {0} device.", deviceName);
                Environment.Exit(-1);
            }

            NuiStreamTypes s = NuiStreamTypes.ColorData;
            for (; s <= NuiStreamTypes.AudioData; s++)
            {
                object val = ReflectionUtilities.InvokeMethod(device, "supportsStreamType", new object[] { s });
                if (val is bool && (bool)val == true)
                {
                    Console.WriteLine("Device {0} supports stream type {1}", deviceName, s);
                }
            }

            List<string> myWords = new List<string> { "hello", "computer", "action" };

            GestureManager gm = new GestureManager(device);
			
			// /*
            Gesture audioGesture = new AudioGesture(device);
            foreach (string w in myWords)
            {
                ((AudioGesture)audioGesture).AddWord(w);
            }

            if (!gm.Add(audioGesture))
            {
                Console.WriteLine("Could not add the gesture {0} to the device {1}. Unsupported gesture.", audioGesture, device);
                Environment.Exit(-1);
            }
			// */

            /*
			Gesture skeletonGesture = new SwipeLeft(device);
            if (!gm.Add(skeletonGesture))
            {
                Console.WriteLine("Could not add the gesture {0} to the device {1}. Unsupported gesture.", skeletonGesture, device);
            }
			*/
			
            Console.WriteLine("You've successfully added a Gesture to the GestureManager!");

            Console.WriteLine("Now the GestureManager will start listening for input.");
            gm.Start();
            List<Gesture> completedGestures;
            for (; ; )
            {
                completedGestures = gm.getCompletedGestures();
                if (completedGestures.Count == 0)
                {
                    //Console.WriteLine("No gesture detected this frame.");
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

        static void Main(string[] args)
        {
            //RunMessagingApplication();
            RunStandaloneTestApplication();
        }
    }
}
