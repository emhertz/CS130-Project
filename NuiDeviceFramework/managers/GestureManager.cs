using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.devices;
using NuiDeviceFramework.datatypes.skeleton.enums;
using NuiDeviceFramework.reflection;

namespace NuiDeviceFramework.managers
{
    public class GestureManager
    {
        private List<Gesture> gestures = new List<Gesture>();
        private List<Boolean> gesturesCompleted = new List<Boolean>();
        private List<Thread> gestureThreads = new List<Thread>();
        private object device;
        private Boolean running;

        public Boolean IsRunning
        {
            get
            {
                return this.running;
            }
        }

        public GestureManager(object d)
        {
            this.device = d;
        }

        public Boolean Add(Gesture g)
        {
            if (!checkDeviceSupportsFeatures(g))
            {
                return false;
            }
            gestures.Add(g);
            gesturesCompleted.Add(false);
            return true;
        }

        public Boolean checkDeviceSupportsFeatures(Gesture g)
        {
            foreach (NuiStreamTypes s in g.getNecessaryStreams())
            {
                object val = ReflectionUtilities.InvokeMethod(device, "supportsStreamType", new object[]{s});
                if (val is bool && (! (bool)val) )
                {
                    return false;
                }
            }
            return true;
        }

        public void Start()
        {
            running = true;
            foreach (Gesture g in gestures)
            {
                Thread t = new Thread(new ThreadStart(g.Start));
                t.Start();
                gestureThreads.Add(t);
            }
        }

        public List<Gesture> getCompletedGestures()
        {
            List<Gesture> result = new List<Gesture>();

            foreach (Gesture g in gestures)
            {
                if (g.GestureDetected)
                {
                    result.Add(g);
                }
                g.GestureDetected = false;
            }

            return result;
        }

        public void Stop()
        {
            running = false;
            foreach (Thread t in gestureThreads)
            {
                if (t.IsAlive)
                {
                    t.Abort();
                }
            }
        }
    }
}
