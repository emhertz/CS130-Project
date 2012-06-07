using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.managers;
using NuiDeviceFramework.reflection;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.gestures.implementations;

namespace MessagingClient
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

        public static string ReceiveMessage(string queuePath)
        {
            string message = "";
            try
            {
                // Connect to the a queue on the local computer.
                MessageQueue myQueue = new MessageQueue(queuePath);

                // Receive and format the message. 
                System.Messaging.Message myMessage = myQueue.Receive();
                myMessage.Formatter = new BinaryMessageFormatter();
                message = (string)myMessage.Body;

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

            return message;
        }

        static void RunMessagingApplication()
        {
            Console.WriteLine("Welcome to the recipient!");
            string recvQueuePath = ".\\Private$\\recvQueue";
            string sendQueuePath = ".\\Private$\\sendQueue";
            string message;
            string ack = "ACK";
            CreateQueue(recvQueuePath);
            CreateQueue(sendQueuePath);
            Console.WriteLine("Welcome to the Cisco Kinect Team 1 Demo Application. (Messaging Version)");
            message = ReceiveMessage(recvQueuePath);
            SendMessage(sendQueuePath, ack);

            Console.WriteLine("First, we will set up a connection to the {0}.", message);
            if (message != "Kinect")
            {
                Console.WriteLine("Unknown device name {0].", message);
                Environment.Exit(-1);
            }
            String deviceName = "NuiDeviceFramework.devices.Kinect";
            String dllPath = "C:\\Users\\Eric\\Documents\\Visual Studio 2010\\Projects\\NuiDeviceFramework\\NuiDeviceFramework\\bin\\Debug\\NuiDeviceFramework.dll";
            object device = DeviceManager.GetConnection(deviceName, dllPath);

            if (device == null)
            {
                Console.WriteLine("Error occurred connecting to the {0} device.", deviceName);
                Environment.Exit(-1);
            }

            NuiStreamTypes s = NuiStreamTypes.ColorData;
            for (; s <= NuiStreamTypes.ObjectData; s++)
            {
                object val = ReflectionUtilities.InvokeMethod(device, "supportsStreamType", new object[] { s });
                if (val is bool && (bool)val == true)
                {
                    Console.WriteLine("Device {0} supports stream type {1}", deviceName, s);
                }
            }

            message = ReceiveMessage(recvQueuePath);
            SendMessage(sendQueuePath, ack);
            GestureManager gm = new GestureManager(device);
            switch (message)
            {
                case "AudioGesture":
                    List<string> myWords = new List<string> { "hello", "computer", "action" };
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

                    Console.WriteLine("You've successfully added a Gesture to the GestureManager!");
                    break;
                default:
                    Console.WriteLine("Unknown gesture type {0}", message);
                    Environment.Exit(-1);
                    break;
            }

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
                SendMessage(sendQueuePath, "Gesture " + ge + " successfully detected!");
                //Console.WriteLine("Gesture {0} successfully detected!", ge);
            }

            Console.WriteLine("The program will now exit.");
        }

        static void Main(string[] args)
        {
            RunMessagingApplication();
        }
    }
}
